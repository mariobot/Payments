using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.InvoiceQuery
{
    /// <summary>
    /// Query to get invoice with all its items (products)
    /// </summary>
    public class GetInvoiceWithItemsQuery : IRequest<InvoiceWithItemsResponse?>
    {
        public int InvoiceId { get; set; }

        public GetInvoiceWithItemsQuery(int invoiceId)
        {
            InvoiceId = invoiceId;
        }
    }

    /// <summary>
    /// Response DTO with invoice and items
    /// </summary>
    public class InvoiceWithItemsResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsActive { get; set; }
        public List<InvoiceItemResponse> Items { get; set; } = new();
    }

    /// <summary>
    /// Invoice item response DTO
    /// </summary>
    public class InvoiceItemResponse
    {
        public int Id { get; set; }
        public int? ProductId { get; set; } // Include ProductId
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class GetInvoiceWithItemsQueryHandler : IRequestHandler<GetInvoiceWithItemsQuery, InvoiceWithItemsResponse?>
    {
        private readonly PaymentsDbContext _context;

        public GetInvoiceWithItemsQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceWithItemsResponse?> Handle(GetInvoiceWithItemsQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items) // Use 'Items' instead of 'InvoiceItems'
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

            if (invoice == null)
                return null;

            return new InvoiceWithItemsResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                Description = invoice.Description,
                TotalAmount = invoice.TotalAmount,
                IsActive = invoice.IsActive,
                Items = invoice.Items.Select(item => new InvoiceItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId, // Include ProductId in response
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Total = item.Total
                }).ToList()
            };
        }
    }
}
