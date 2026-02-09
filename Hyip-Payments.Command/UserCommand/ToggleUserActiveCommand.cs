using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.UserCommand
{
    // MediatR command to toggle user active status
    public class ToggleUserActiveCommand : IRequest<bool>
    {
        public int UserId { get; }
        public ToggleUserActiveCommand(int userId) => UserId = userId;
    }

    // Handler
    public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public ToggleUserActiveCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return false;

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
