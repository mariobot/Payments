using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceCommand
{
    /// <summary>
    /// DTO for invoice (without navigation properties to avoid circular references)
    /// </summary>
    public class InvoiceDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedByUserId { get; set; } // User who created this invoice
    }

    /// <summary>
    /// Command to create an invoice with multiple products in a single transaction
    /// </summary>
    public class AddInvoiceWithProductsCommand : IRequest<InvoiceWithItemsDto>
    {
        public InvoiceDto Invoice { get; set; } = null!;
        public List<InvoiceItemDto> Items { get; set; } = new();
        public string? CreatedByUserId { get; set; } // Set by controller from authenticated user
    }

    /// <summary>
    /// DTO for invoice item (product)
    /// </summary>
    public class InvoiceItemDto
    {
        public int ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// DTO for invoice with items response
    /// </summary>
    public class InvoiceWithItemsDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<InvoiceItemModel> Items { get; set; } = new();
    }

    public class AddInvoiceWithProductsCommandHandler : IRequestHandler<AddInvoiceWithProductsCommand, InvoiceWithItemsDto>
    {
        private readonly PaymentsDbContext _context;

        public AddInvoiceWithProductsCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceWithItemsDto> Handle(AddInvoiceWithProductsCommand request, CancellationToken cancellationToken)
        {
            // Use the execution strategy to handle retries and transactions
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // Start transaction within the execution strategy
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // 1. Create the invoice from DTO
                    var invoice = new InvoiceModel
                    {
                        InvoiceNumber = request.Invoice.InvoiceNumber,
                        InvoiceDate = request.Invoice.InvoiceDate,
                        Description = request.Invoice.Description,
                        TotalAmount = request.Invoice.TotalAmount,
                        IsActive = request.Invoice.IsActive,
                        CreatedByUserId = request.CreatedByUserId // Set the user who created this invoice
                    };

                    _context.Invoices.Add(invoice);
                    await _context.SaveChangesAsync(cancellationToken);

                    // 2. Add invoice items
                    var invoiceItems = new List<InvoiceItemModel>();
                    decimal totalAmount = 0;

                    foreach (var item in request.Items)
                    {
                        // Validate product exists
                        var productExists = await _context.Products
                            .AnyAsync(p => p.Id == item.ProductId, cancellationToken);

                        if (!productExists)
                        {
                            throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
                        }

                        var invoiceItem = new InvoiceItemModel
                        {
                            InvoiceId = invoice.Id,
                            ProductId = item.ProductId > 0 ? item.ProductId : null, // Save ProductId if provided
                            ItemName = item.ItemName,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };

                        _context.InvoiceItems.Add(invoiceItem);
                        invoiceItems.Add(invoiceItem);
                        totalAmount += invoiceItem.Total;
                    }

                    // 3. Update invoice total
                    invoice.TotalAmount = totalAmount;
                    await _context.SaveChangesAsync(cancellationToken);

                    // 4. Commit transaction
                    await transaction.CommitAsync(cancellationToken);

                    return new InvoiceWithItemsDto
                    {
                        InvoiceId = invoice.Id,
                        InvoiceNumber = invoice.InvoiceNumber,
                        TotalAmount = totalAmount,
                        Items = invoiceItems
                    };
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}
