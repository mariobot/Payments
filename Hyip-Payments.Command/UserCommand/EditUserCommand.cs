using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.UserCommand
{
    // MediatR command to edit a user record
    public class EditUserCommand : IRequest<UserModel?>
    {
        public UserModel User { get; }
        public EditUserCommand(UserModel user) => User = user;
    }

    // Handler
    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, UserModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditUserCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.User.Id, cancellationToken);

            if (existing == null)
                return null;

            // Update properties
            existing.Username = request.User.Username;
            existing.Email = request.User.Email;
            existing.PasswordHash = request.User.PasswordHash;
            existing.FirstName = request.User.FirstName;
            existing.LastName = request.User.LastName;
            existing.IsActive = request.User.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
