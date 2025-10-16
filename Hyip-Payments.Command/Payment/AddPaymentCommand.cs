using MediatR;
using Hyip_Payments.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Command.Payment
{
    // Interface for AddPaymentCommand
    public interface IAddPaymentCommand : IRequest<PaymentModel>
    {
        PaymentModel Payment { get; }
    }

    // Implementation of AddPaymentCommand
    public class AddPaymentCommand : IAddPaymentCommand
    {
        public PaymentModel Payment { get; }

        public AddPaymentCommand(PaymentModel payment)
        {
            Payment = payment;
        }
    }

    // Handler for AddPaymentCommand
    public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, PaymentModel>
    {
        // Inject your data context or repository here as needed
        // private readonly PaymentsDbContext _context;

        // public AddPaymentCommandHandler(PaymentsDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<PaymentModel> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
        {
            // TODO: Add your logic to persist the payment
            // Example:
            // _context.Payments.Add(request.Payment);
            // await _context.SaveChangesAsync(cancellationToken);
            // return request.Payment;

            // For demonstration, just return the payment
            return await Task.FromResult(request.Payment);
        }
    }
}
