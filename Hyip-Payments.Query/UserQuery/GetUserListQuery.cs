using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.UserQuery
{
    // MediatR query to get all users
    public class GetUserListQuery : IRequest<List<UserModel>>
    {
    }

    // Handler
    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, List<UserModel>>
    {
        private readonly PaymentsDbContext _context;

        public GetUserListQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserModel>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
