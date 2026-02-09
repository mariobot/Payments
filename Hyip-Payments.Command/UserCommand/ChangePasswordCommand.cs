using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.UserCommand
{
    // MediatR command to change user password
    public class ChangePasswordCommand : IRequest<bool>
    {
        public int UserId { get; }
        public string NewPassword { get; }
        
        public ChangePasswordCommand(int userId, string newPassword)
        {
            UserId = userId;
            NewPassword = newPassword;
        }
    }

    // Handler
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public ChangePasswordCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return false;

            // Update password (Note: In production, hash this password!)
            user.PasswordHash = request.NewPassword;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
