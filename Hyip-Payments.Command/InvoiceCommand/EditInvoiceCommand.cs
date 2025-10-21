using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceCommand
{
    // MediatR command to edit an invoice
    public class EditInvoiceCommand : IRequest<InvoiceModel?>
    {
        public InvoiceModel Invoice { get; }
        public EditInvoiceCommand(InvoiceModel invoice) => Invoice = invoice;
    }

    // Handler
    public class EditInvoiceCommandHandler : IRequestHandler<EditInvoiceCommand, InvoiceModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditInvoiceCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceModel?> Handle(EditInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == request.Invoice.Id, cancellationToken);

            if (existing == null)
                return null;

            existing.InvoiceNumber = request.Invoice.InvoiceNumber;
            existing.InvoiceDate = request.Invoice.InvoiceDate;
            existing.Description = request.Invoice.Description;
            existing.TotalAmount = request.Invoice.TotalAmount;

            // Update items (simple replace, for more complex logic handle add/update/delete individually)
            if (request.Invoice.Items != null)
            {
                // Remove old items
                _context.InvoiceItems.RemoveRange(existing.Items);
                // Add new items
                existing.Items = request.Invoice.Items;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
