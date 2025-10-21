using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CoinQuery
{
    // Query
    public class GetCointByIdQuery : IRequest<CoinModel?>
    {
        public int Id { get; }

        public GetCointByIdQuery(int id)
        {
            Id = id;
        }
    }

    // Handler
    public class GetCointByIdQueryHandler : IRequestHandler<GetCointByIdQuery, CoinModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetCointByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CoinModel?> Handle(GetCointByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Coins
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        }
    }
}
