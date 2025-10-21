using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.PaymentCommand
{
    // MediatR command to edit a payment record
    public class EditPaymentCommand : IRequest<PaymentModel?>
    {
        public PaymentModel Payment { get; }
        public EditPaymentCommand(PaymentModel payment) => Payment = payment;
    }

    // Handler
    public class EditPaymentCommandHandler : IRequestHandler<EditPaymentCommand, PaymentModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditPaymentCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentModel?> Handle(EditPaymentCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == request.Payment.Id, cancellationToken);

            if (existing == null)
                return null;

            // Update properties as needed
            existing.Amount = request.Payment.Amount;
            existing.Currency = request.Payment.Currency;
            existing.Date = request.Payment.Date;
            existing.Description = request.Payment.Description;
            existing.Payer = request.Payment.Payer;
            existing.InvoiceId = request.Payment.InvoiceId;
            existing.Date = DateTime.UtcNow;
            existing.Status = request.Payment.Status;
            //existing.PaymentDate = request.Payment.PaymentDate;
            // Add other properties as needed

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
