using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.PaymentCommand
{
    /// <summary>
    /// Command to edit/update a payment transaction
    /// </summary>
    public class EditPaymentTransactionCommand : IRequest<PaymentTransactionModel?>
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = "Pending";
        public int WalletId { get; set; }
        public int PaymentMethodId { get; set; }
        public int? InvoiceId { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }
    }

    public class EditPaymentTransactionCommandHandler : IRequestHandler<EditPaymentTransactionCommand, PaymentTransactionModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditPaymentTransactionCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransactionModel?> Handle(EditPaymentTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(pt => pt.Id == request.Id, cancellationToken);

            if (transaction == null)
                return null;

            // Update properties
            transaction.Amount = request.Amount;
            transaction.TransactionDate = request.TransactionDate;
            transaction.Status = request.Status;
            transaction.WalletId = request.WalletId;
            transaction.PaymentMethodId = request.PaymentMethodId;
            transaction.InvoiceId = request.InvoiceId;
            transaction.Reference = request.Reference;
            transaction.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);
            return transaction;
        }
    }
}
