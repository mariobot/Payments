using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomReportQuery
{
    public class GetCustomReportByIdQueryHandler : IRequestHandler<GetCustomReportByIdQuery, CustomReportModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetCustomReportByIdQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<CustomReportModel?> Handle(GetCustomReportByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.CustomReports
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        }
    }
}
