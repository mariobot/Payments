using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CountryQuery
{
    // Command (request)
    public class GetCountryListQuery : IRequest<List<CountryModel?>>
    {
        public GetCountryListQuery()
        {
            
        }
    }

    // Handler
    public class GetCountryListQueryHandler : IRequestHandler<GetCountryListQuery, List<CountryModel?>>
    {
        private readonly PaymentsDbContext _context;

        public GetCountryListQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CountryModel?>> Handle(GetCountryListQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Countries.ToListAsync();

            return data;
        }
    }
}
