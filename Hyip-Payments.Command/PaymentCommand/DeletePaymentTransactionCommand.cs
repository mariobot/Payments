using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.PaymentCommand
{
    /// <summary>
    /// Command to delete a payment transaction
    /// </summary>
    public class DeletePaymentTransactionCommand : IRequest<bool>
    {
        public int Id { get; }

        public DeletePaymentTransactionCommand(int id)
        {
            Id = id;
        }
    }

    public class DeletePaymentTransactionCommandHandler : IRequestHandler<DeletePaymentTransactionCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeletePaymentTransactionCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeletePaymentTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(pt => pt.Id == request.Id, cancellationToken);

            if (transaction == null)
                return false;

            _context.PaymentTransactions.Remove(transaction);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
