using MediatR;
using Hyip_Payments.Models;

namespace Hyip_Payments.Command.Payment
{
    public interface IAddPaymentCommand : IRequest<PaymentModel>
    {
        PaymentModel Payment { get; }
    }
}
