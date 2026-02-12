using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.PaymentMethodCommand
{
    /// <summary>
    /// Command to add a new payment method
    /// </summary>
    public class AddPaymentMethodCommand : IRequest<PaymentMethodModel>
    {
        public PaymentMethodModel PaymentMethod { get; }

        public AddPaymentMethodCommand(PaymentMethodModel paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }
    }

    public class AddPaymentMethodCommandHandler : IRequestHandler<AddPaymentMethodCommand, PaymentMethodModel>
    {
        private readonly PaymentsDbContext _context;

        public AddPaymentMethodCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodModel> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            _context.PaymentMethods.Add(request.PaymentMethod);
            await _context.SaveChangesAsync(cancellationToken);
            return request.PaymentMethod;
        }
    }
}
