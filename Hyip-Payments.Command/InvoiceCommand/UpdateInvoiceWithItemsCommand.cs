using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceCommand
{
    /// <summary>
    /// DTO for updating invoice (without navigation properties)
    /// </summary>
    public class UpdateInvoiceDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public int? CustomerId { get; set; } // Link to customer
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for updating invoice item
    /// </summary>
    public class UpdateInvoiceItemDto
    {
        public int? Id { get; set; } // Null for new items
        public int ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// Command to update an invoice with its items
    /// </summary>
    public class UpdateInvoiceWithItemsCommand : IRequest<InvoiceWithItemsDto>
    {
        public int InvoiceId { get; set; }
        public UpdateInvoiceDto Invoice { get; set; } = null!;
        public List<UpdateInvoiceItemDto> Items { get; set; } = new();
    }

    public class UpdateInvoiceWithItemsCommandHandler : IRequestHandler<UpdateInvoiceWithItemsCommand, InvoiceWithItemsDto>
    {
        private readonly PaymentsDbContext _context;

        public UpdateInvoiceWithItemsCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceWithItemsDto> Handle(UpdateInvoiceWithItemsCommand request, CancellationToken cancellationToken)
        {
            // Use the execution strategy to handle retries and transactions
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                // Start transaction within the execution strategy
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    // 1. Get existing invoice with items
                    var invoice = await _context.Invoices
                        .Include(i => i.Items)
                        .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

                    if (invoice == null)
                    {
                        throw new InvalidOperationException($"Invoice with ID {request.InvoiceId} not found");
                    }

                    // 2. Update invoice properties
                    invoice.InvoiceNumber = request.Invoice.InvoiceNumber;
                    invoice.InvoiceDate = request.Invoice.InvoiceDate;
                    invoice.Description = request.Invoice.Description;
                    invoice.CustomerId = request.Invoice.CustomerId; // Update CustomerId
                    invoice.IsActive = request.Invoice.IsActive;

                    // 3. Remove all existing items (simpler than trying to match and update)
                    _context.InvoiceItems.RemoveRange(invoice.Items);
                    await _context.SaveChangesAsync(cancellationToken);

                    // 4. Add new items
                    var invoiceItems = new List<InvoiceItemModel>();
                    decimal totalAmount = 0;

                    foreach (var itemDto in request.Items)
                    {
                        // Validate product exists if ProductId is provided
                        if (itemDto.ProductId > 0)
                        {
                            var productExists = await _context.Products
                                .AnyAsync(p => p.Id == itemDto.ProductId, cancellationToken);

                            if (!productExists)
                            {
                                throw new InvalidOperationException($"Product with ID {itemDto.ProductId} not found");
                            }
                        }

                        var invoiceItem = new InvoiceItemModel
                        {
                            InvoiceId = invoice.Id,
                            ProductId = itemDto.ProductId > 0 ? itemDto.ProductId : null, // Save ProductId if provided
                            ItemName = itemDto.ItemName,
                            Quantity = itemDto.Quantity,
                            UnitPrice = itemDto.UnitPrice
                        };

                        _context.InvoiceItems.Add(invoiceItem);
                        invoiceItems.Add(invoiceItem);
                        totalAmount += invoiceItem.Total;
                    }

                    // 5. Update invoice total
                    invoice.TotalAmount = totalAmount;
                    await _context.SaveChangesAsync(cancellationToken);

                    // 6. Commit transaction
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
