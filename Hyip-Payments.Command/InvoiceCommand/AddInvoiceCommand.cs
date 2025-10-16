using MediatR;
using Hyip_Payments.Models;

namespace Hyip_Payments.Command.InvoiceCommand
{
    // Interface for AddInvoiceCommand
    public interface IAddInvoiceCommand : IRequest<InvoiceModel>
    {
        InvoiceModel Invoice { get; }
    }

    // Implementation of AddInvoiceCommand
    public class AddInvoiceCommand : IAddInvoiceCommand
    {
        public InvoiceModel Invoice { get; }

        public AddInvoiceCommand(InvoiceModel invoice)
        {
            Invoice = invoice;
        }
    }

    // Handler for AddInvoiceCommand
    public class AddInvoiceCommandHandler : IRequestHandler<AddInvoiceCommand, InvoiceModel>
    {
        // Inject your data context or repository here as needed
        // private readonly PaymentsDbContext _context;

        // public AddInvoiceCommandHandler(PaymentsDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<InvoiceModel> Handle(AddInvoiceCommand request, CancellationToken cancellationToken)
        {
            // TODO: Add your logic to persist the invoice
            // Example:
            // _context.Invoices.Add(request.Invoice);
            // await _context.SaveChangesAsync(cancellationToken);
            // return request.Invoice;

            // For demonstration, just return the invoice
            return await Task.FromResult(request.Invoice);
        }
    }
}
