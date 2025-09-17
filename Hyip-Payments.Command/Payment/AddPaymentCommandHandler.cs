using MediatR;
using Hyip_Payments.Models;
//using Hyip_Payments.Server.Data;

namespace Hyip_Payments.Command.Payment
{
    public class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, PaymentModel>
    {
        //private readonly ApplicationDbContext _context;

        public AddPaymentCommandHandler(/*ApplicationDbContext context*/)
        {
            //ontext = context;
        }

        public async Task<PaymentModel> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
        {
            /*_context.Payments.Add(request.Payment);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Payment;*/

            return await Task.FromResult(request.Payment);
        }
    }
}
