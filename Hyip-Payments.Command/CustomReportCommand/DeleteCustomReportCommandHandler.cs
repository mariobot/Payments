using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public class DeleteCustomReportCommandHandler : IRequestHandler<DeleteCustomReportCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteCustomReportCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCustomReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _context.CustomReports
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (report == null)
                return false;

            _context.CustomReports.Remove(report);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
