using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;

namespace Hyip_Payments.Command.PaymentCommand
{
    // Command
    public class AddPaymentTransactionCommand : IRequest<PaymentTransactionModel>
    {
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; } = "Pending";
        public int WalletId { get; set; }
        public int PaymentMethodId { get; set; }
        public int? InvoiceId { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }
    }

    // Handler
    public class AddPaymentTransactionCommandHandler : IRequestHandler<AddPaymentTransactionCommand, PaymentTransactionModel>
    {
        private readonly PaymentsDbContext _context;

        public AddPaymentTransactionCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransactionModel> Handle(AddPaymentTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = new PaymentTransactionModel
            {
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                Status = request.Status,
                WalletId = request.WalletId,
                PaymentMethodId = request.PaymentMethodId,
                InvoiceId = request.InvoiceId,
                Reference = request.Reference,
                Description = request.Description
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return transaction;
        }
    }
}
