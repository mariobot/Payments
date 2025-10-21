using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceItemCommand
{
    // MediatR command to delete an invoice item by ID
    public class DeleteInvoiceItemCommand : IRequest<bool>
    {
        public int InvoiceItemId { get; }
        public DeleteInvoiceItemCommand(int invoiceItemId) => InvoiceItemId = invoiceItemId;
    }

    // Handler
    public class DeleteInvoiceItemCommandHandler : IRequestHandler<DeleteInvoiceItemCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteInvoiceItemCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteInvoiceItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.InvoiceItems
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceItemId, cancellationToken);

            if (item == null)
                return false;

            _context.InvoiceItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
