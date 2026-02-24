using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public class SaveCustomReportCommandHandler : IRequestHandler<SaveCustomReportCommand, CustomReportModel>
    {
        private readonly PaymentsDbContext _context;

        public SaveCustomReportCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<CustomReportModel> Handle(SaveCustomReportCommand request, CancellationToken cancellationToken)
        {
            _context.CustomReports.Add(request.Report);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Report;
        }
    }
}
