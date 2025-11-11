using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.UserCommand
{
    // Command
    public class LoginUserCommand : IRequest<UserModel?>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public LoginUserCommand() { }

        public LoginUserCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }

    // Handler
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserModel?>
    {
        private readonly PaymentsDbContext _context;

        public LoginUserCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            // Find user by email
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null)
                return null;

            // Validate password (in production, use proper password hashing like BCrypt)
            if (user.PasswordHash != request.Password)
                return null;

            // Check if user is active
            if (!user.IsActive)
                return null;

            return user;
        }
    }
}
