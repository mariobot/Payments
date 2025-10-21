using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.PaymentCommand
{
    // MediatR command to delete a payment by ID
    public class DeletePaymentCommand : IRequest<bool>
    {
        public Guid PaymentId { get; }
        public DeletePaymentCommand(Guid paymentId) => PaymentId = paymentId;
    }

    // Handler
    public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeletePaymentCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == request.PaymentId);

            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
