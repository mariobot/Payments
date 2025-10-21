using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.UserQuery
{
    // MediatR query to get a user by ID
    public class GetUserByIdQuery : IRequest<UserModel?>
    {
        public int UserId { get; }
        public GetUserByIdQuery(int userId) => UserId = userId;
    }

    // Handler
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetUserByIdQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserModel?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        }
    }
}
