using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceCommand
{
    // MediatR command to add an invoice
    public class AddInvoiceCommand : IRequest<InvoiceModel>
    {
        public InvoiceModel Invoice { get; }
        public AddInvoiceCommand(InvoiceModel invoice) => Invoice = invoice;
    }

    // Optional interface for abstraction/testing
    public interface IAddInvoiceCommandHandler
    {
        Task<InvoiceModel> Handle(AddInvoiceCommand request, CancellationToken cancellationToken);
    }

    // Handler
    public class AddInvoiceCommandHandler : IRequestHandler<AddInvoiceCommand, InvoiceModel>, IAddInvoiceCommandHandler
    {
        private readonly PaymentsDbContext _context;

        public AddInvoiceCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceModel> Handle(AddInvoiceCommand request, CancellationToken cancellationToken)
        {
            // Attach items if present
            if (request.Invoice.Items != null && request.Invoice.Items.Count > 0)
            {
                foreach (var item in request.Invoice.Items)
                {
                    _context.InvoiceItems.Add(item);
                }
            }

            _context.Invoices.Add(request.Invoice);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Invoice;
        }
    }
}
