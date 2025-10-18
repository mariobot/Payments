using MediatR;
using Hyip_Payments.Models;


namespace Hyip_Payments.Query.InvoiceQuery
{
    // Query to get all invoices
    public class GetAllInvoicesQuery : IRequest<List<InvoiceModel>>
    {
    }

    // Query to get invoice by ID
    public class GetInvoiceByIdQuery : IRequest<InvoiceModel?>
    {
        public int Id { get; }

        public GetInvoiceByIdQuery(int id)
        {
            Id = id;
        }
    }

    // Handler for GetAllInvoicesQuery
    public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, List<InvoiceModel>>
    {
        // Inject your data context or repository here as needed
        // private readonly PaymentsDbContext _context;

        // public GetAllInvoicesQueryHandler(PaymentsDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<List<InvoiceModel>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
        {
            // TODO: Replace with actual database query
            // Example:
            // return await _context.Invoices.Include(i => i.Items).ToListAsync(cancellationToken);

            // For demonstration, returning sample data
            return await Task.FromResult(new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    Id = 1,
                    InvoiceNumber = "INV-001",
                    InvoiceDate = DateTime.UtcNow.AddDays(-2),
                    Description = "Web development services",
                    TotalAmount = 1200,
                    Items = new List<InvoiceItemModel>
                    {
                        new InvoiceItemModel { ItemName = "Design", Quantity = 1, UnitPrice = 500 },
                        new InvoiceItemModel { ItemName = "Development", Quantity = 1, UnitPrice = 700 }
                    }
                },
                new InvoiceModel
                {
                    Id = 2,
                    InvoiceNumber = "INV-002",
                    InvoiceDate = DateTime.UtcNow.AddDays(-1),
                    Description = "Consulting",
                    TotalAmount = 300,
                    Items = new List<InvoiceItemModel>
                    {
                        new InvoiceItemModel { ItemName = "Consulting Hours", Quantity = 3, UnitPrice = 100 }
                    }
                }
            });
        }
    }

    // Handler for GetInvoiceByIdQuery
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceModel?>
    {
        // Inject your data context or repository here as needed
        // private readonly PaymentsDbContext _context;

        // public GetInvoiceByIdQueryHandler(PaymentsDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<InvoiceModel?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: Replace with actual database query
            // Example:
            // return await _context.Invoices
            //     .Include(i => i.Items)
            //     .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            // For demonstration, returning sample data
            var invoices = new List<InvoiceModel>
            {
                new InvoiceModel
                {
                    Id = 1,
                    InvoiceNumber = "INV-001",
                    InvoiceDate = DateTime.UtcNow.AddDays(-2),
                    Description = "Web development services",
                    TotalAmount = 1200,
                    Items = new List<InvoiceItemModel>
                    {
                        new InvoiceItemModel { ItemName = "Design", Quantity = 1, UnitPrice = 500 },
                        new InvoiceItemModel { ItemName = "Development", Quantity = 1, UnitPrice = 700 }
                    }
                },
                new InvoiceModel
                {
                    Id = 2,
                    InvoiceNumber = "INV-002",
                    InvoiceDate = DateTime.UtcNow.AddDays(-1),
                    Description = "Consulting",
                    TotalAmount = 300,
                    Items = new List<InvoiceItemModel>
                    {
                        new InvoiceItemModel { ItemName = "Consulting Hours", Quantity = 3, UnitPrice = 100 }
                    }
                }
            };

            return await Task.FromResult(invoices.FirstOrDefault(i => i.Id == request.Id));
        }
    }
}
