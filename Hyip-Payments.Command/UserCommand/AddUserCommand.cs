using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.UserCommand
{
    // MediatR command to add a user
    public class AddUserCommand : IRequest<UserModel>
    {
        public UserModel User { get; }
        public AddUserCommand(UserModel user) => User = user;
    }

    // Handler
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, UserModel>
    {
        private readonly PaymentsDbContext _context;

        public AddUserCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            _context.Users.Add(request.User);
            await _context.SaveChangesAsync(cancellationToken);
            return request.User;
        }
    }
}
