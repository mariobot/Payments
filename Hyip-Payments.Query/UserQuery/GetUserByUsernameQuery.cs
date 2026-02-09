using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.UserQuery
{
    // MediatR query to get a user by username
    public class GetUserByUsernameQuery : IRequest<UserModel?>
    {
        public string Username { get; }
        public GetUserByUsernameQuery(string username) => Username = username;
    }

    // Handler
    public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, UserModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetUserByUsernameQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);
        }
    }
}
