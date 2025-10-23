using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.MoneyQuery
{
    // Query to get all money records
    public class GetMoneyQuery : IRequest<List<MoneyModel>>
    {
    }

    // Handler for GetMoneyQuery
    public class GetMoneyQueryHandler : IRequestHandler<GetMoneyQuery, List<MoneyModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetMoneyQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MoneyModel>> Handle(GetMoneyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tmp = await _dbContext.Money
                .ToListAsync(cancellationToken);

                return await _dbContext.Money
                .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
