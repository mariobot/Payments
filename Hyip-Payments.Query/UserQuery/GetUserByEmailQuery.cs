using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.UserQuery
{
    // MediatR query to get a user by email
    public class GetUserByEmailQuery : IRequest<UserModel?>
    {
        public string Email { get; }
        public GetUserByEmailQuery(string email) => Email = email;
    }

    // Handler
    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetUserByEmailQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        }
    }
}
