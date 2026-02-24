using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public class IncrementReportRunCountCommandHandler : IRequestHandler<IncrementReportRunCountCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public IncrementReportRunCountCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(IncrementReportRunCountCommand request, CancellationToken cancellationToken)
        {
            var report = await _context.CustomReports
                .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken);

            if (report == null)
                return false;

            report.RunCount++;
            report.LastRunDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
