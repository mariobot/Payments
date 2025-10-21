using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CountryQuery
{
    // Query
    public class GetCountryByIdQuery : IRequest<CountryModel?>
    {
        public int Id { get; }

        public GetCountryByIdQuery(int id)
        {
            Id = id;
        }
    }

    // Handler
    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, CountryModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetCountryByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CountryModel?> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        }
    }
}
