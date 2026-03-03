using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Hyip_Payments.Services;
using MediatR;

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
        private readonly InvoiceNumberService _invoiceNumberService;
        private readonly IMediator _mediator;

        public AddInvoiceCommandHandler(
            PaymentsDbContext context,
            InvoiceNumberService invoiceNumberService,
            IMediator mediator)
        {
            _context = context;
            _invoiceNumberService = invoiceNumberService;
            _mediator = mediator;
        }

        public async Task<InvoiceModel> Handle(AddInvoiceCommand request, CancellationToken cancellationToken)
        {
            // Auto-generate invoice number if not provided or empty
            if (string.IsNullOrWhiteSpace(request.Invoice.InvoiceNumber))
            {
                request.Invoice.InvoiceNumber = await _invoiceNumberService.GenerateNextInvoiceNumberAsync();
            }

            // Attach items if present
            if (request.Invoice.Items != null && request.Invoice.Items.Count > 0)
            {
                foreach (var item in request.Invoice.Items)
                {
                    item.InvoiceId = request.Invoice.Id; // Link item to invoice
                    _context.InvoiceItems.Add(item);
                }
            }

            _context.Invoices.Add(request.Invoice);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish event to trigger customer balance update
            if (request.Invoice.CustomerId.HasValue)
            {
                await _mediator.Publish(new InvoiceCreatedEvent(request.Invoice.Id, request.Invoice.CustomerId), cancellationToken);
            }

            return request.Invoice;
        }
    }
}
