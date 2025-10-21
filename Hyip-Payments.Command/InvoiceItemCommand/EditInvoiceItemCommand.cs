using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.InvoiceItemCommand
{
    // MediatR command to edit an invoice item
    public class EditInvoiceItemCommand : IRequest<InvoiceItemModel?>
    {
        public InvoiceItemModel InvoiceItem { get; }
        public EditInvoiceItemCommand(InvoiceItemModel invoiceItem) => InvoiceItem = invoiceItem;
    }

    // Handler
    public class EditInvoiceItemCommandHandler : IRequestHandler<EditInvoiceItemCommand, InvoiceItemModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditInvoiceItemCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemModel?> Handle(EditInvoiceItemCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.InvoiceItems
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceItem.Id, cancellationToken);

            if (existing == null)
                return null;

            existing.ItemName = request.InvoiceItem.ItemName;
            existing.Quantity = request.InvoiceItem.Quantity;
            existing.UnitPrice = request.InvoiceItem.UnitPrice;
            existing.InvoiceId = request.InvoiceItem.InvoiceId;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
