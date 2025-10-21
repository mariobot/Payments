using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.PaymentCommand
{
    // Command (request)
    public class AddPaymentCommand : IRequest<PaymentModel>
    {
        public PaymentModel Payment { get; }

        public AddPaymentCommand(PaymentModel payment)
        {
            Payment = payment;
        }
    }

    // Interface for abstraction (optional, for DI or testing)
    public interface IAddPaymentCommandHandler
    {
        Task<PaymentModel> Handle(AddPaymentCommand request, CancellationToken cancellationToken);
    }

    // Handler
    public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, PaymentModel>, IAddPaymentCommandHandler
    {
        private readonly PaymentsDbContext _context;

        public AddPaymentCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentModel> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
        {
            _context.Payments.Add(request.Payment);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Payment;
        }
    }
}
