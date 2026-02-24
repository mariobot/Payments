using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public class UpdateCustomReportCommandHandler : IRequestHandler<UpdateCustomReportCommand, CustomReportModel>
    {
        private readonly PaymentsDbContext _context;

        public UpdateCustomReportCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<CustomReportModel> Handle(UpdateCustomReportCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.CustomReports
                .FirstOrDefaultAsync(r => r.Id == request.Report.Id, cancellationToken);

            if (existing == null)
                throw new InvalidOperationException($"Custom report with ID {request.Report.Id} not found.");

            existing.ReportName = request.Report.ReportName;
            existing.Description = request.Report.Description;
            existing.ReportType = request.Report.ReportType;
            existing.DataSource = request.Report.DataSource;
            existing.ConfigurationJson = request.Report.ConfigurationJson;
            existing.IsPublic = request.Report.IsPublic;
            existing.IsActive = request.Report.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
