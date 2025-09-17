using MediatR;
using Hyip_Payments.Models;

namespace Hyip_Payments.Command.Payment
{
    public class AddPaymentCommand : IRequest<PaymentModel>
    {
        public PaymentModel Payment { get; }

        public AddPaymentCommand(PaymentModel payment)
        {
            Payment = payment;
        }
    }
}
