using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.PaymentMethodCommand
{
    /// <summary>
    /// Command to edit an existing payment method
    /// </summary>
    public class EditPaymentMethodCommand : IRequest<PaymentMethodModel?>
    {
        public PaymentMethodModel PaymentMethod { get; }

        public EditPaymentMethodCommand(PaymentMethodModel paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }
    }

    public class EditPaymentMethodCommandHandler : IRequestHandler<EditPaymentMethodCommand, PaymentMethodModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditPaymentMethodCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodModel?> Handle(EditPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.Id == request.PaymentMethod.Id, cancellationToken);

            if (existing == null)
                return null;

            // Update properties
            existing.Name = request.PaymentMethod.Name;
            existing.Description = request.PaymentMethod.Description;
            existing.IsActive = request.PaymentMethod.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
