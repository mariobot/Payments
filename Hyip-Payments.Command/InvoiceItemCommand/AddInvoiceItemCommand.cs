using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.InvoiceItemCommand
{
    // MediatR command to add an invoice item
    public class AddInvoiceItemCommand : IRequest<InvoiceItemModel>
    {
        public InvoiceItemModel InvoiceItem { get; }
        public AddInvoiceItemCommand(InvoiceItemModel invoiceItem) => InvoiceItem = invoiceItem;
    }

    // Handler
    public class AddInvoiceItemCommandHandler : IRequestHandler<AddInvoiceItemCommand, InvoiceItemModel>
    {
        private readonly PaymentsDbContext _context;

        public AddInvoiceItemCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemModel> Handle(AddInvoiceItemCommand request, CancellationToken cancellationToken)
        {
            _context.InvoiceItems.Add(request.InvoiceItem);
            await _context.SaveChangesAsync(cancellationToken);
            return request.InvoiceItem;
        }
    }
}
