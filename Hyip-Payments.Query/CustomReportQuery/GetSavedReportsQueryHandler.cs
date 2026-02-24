using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomReportQuery
{
    public class GetSavedReportsQueryHandler : IRequestHandler<GetSavedReportsQuery, List<CustomReportModel>>
    {
        private readonly PaymentsDbContext _context;

        public GetSavedReportsQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomReportModel>> Handle(GetSavedReportsQuery request, CancellationToken cancellationToken)
        {
            return await _context.CustomReports
                .Where(r => r.IsActive && 
                       (r.CreatedByUserId == request.UserId || 
                        (request.IncludePublic && r.IsPublic)))
                .OrderByDescending(r => r.LastRunDate ?? r.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
