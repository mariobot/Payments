using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceCommand
{
    // MediatR command to delete an invoice by ID
    public class DeleteInvoiceCommand : IRequest<bool>
    {
        public int InvoiceId { get; }
        public DeleteInvoiceCommand(int invoiceId) => InvoiceId = invoiceId;
    }

    // Handler
    public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteInvoiceCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
        {
            // Optionally include items if you want to delete them as well
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

            if (invoice == null)
                return false;

            // Remove related items if needed
            if (invoice.Items != null && invoice.Items.Count > 0)
            {
                _context.InvoiceItems.RemoveRange(invoice.Items);
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
