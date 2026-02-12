using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.PaymentMethodCommand
{
    /// <summary>
    /// Command to delete a payment method
    /// </summary>
    public class DeletePaymentMethodCommand : IRequest<bool>
    {
        public int Id { get; }

        public DeletePaymentMethodCommand(int id)
        {
            Id = id;
        }
    }

    public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeletePaymentMethodCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (paymentMethod == null)
                return false;

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
