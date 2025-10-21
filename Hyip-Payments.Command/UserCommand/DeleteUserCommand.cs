using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.UserCommand
{
    // MediatR command to delete a user by ID
    public class DeleteUserCommand : IRequest<bool>
    {
        public int UserId { get; }
        public DeleteUserCommand(int userId) => UserId = userId;
    }

    // Handler
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteUserCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
