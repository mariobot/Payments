using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.MoneyQuery
{
    // Query to get a money record by ID
    public class GetMoneyByIdQuery : IRequest<MoneyModel?>
    {
        public int Id { get; }

        public GetMoneyByIdQuery(int id)
        {
            Id = id;
        }
    }

    // Handler for GetMoneyByIdQuery
    public class GetMoneyByIdQueryHandler : IRequestHandler<GetMoneyByIdQuery, MoneyModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetMoneyByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MoneyModel?> Handle(GetMoneyByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Money
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        }
    }
}
