using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.UserCommand
{
    // Command
    public class RegisterUserCommand : IRequest<UserModel>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = "User"; // Default role
        public bool EmailConfirmed { get; set; } = false;

        public RegisterUserCommand() { }

        public RegisterUserCommand(string email, string password, string username, string roleName = "User")
        {
            Email = email;
            Password = password;
            Username = username;
            RoleName = roleName;
        }
    }

    // Handler
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserModel>
    {
        private readonly PaymentsDbContext _context;

        public RegisterUserCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Create user
            var user = new UserModel
            {
                Email = request.Email,
                PasswordHash = request.Password,
                Username = request.Username,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // Find or create role
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);
            if (role == null)
            {
                role = new RoleModel { Name = request.RoleName };
                _context.Roles.Add(role);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Assign role to user
            var userRole = new UserRoleModel
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            _context.UserRoles.Add(userRole);

            // Create application user
            var appUser = new UserApplicationModel
            {
                UserId = user.Id,
                EmailConfirmed = request.EmailConfirmed,
                UserTenantId = 1, // Assign tenant 1 (HomeCompany)
                Email = request.Email,
                Password = request.Password,
            };
            _context.UserApplications.Add(appUser);

            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
