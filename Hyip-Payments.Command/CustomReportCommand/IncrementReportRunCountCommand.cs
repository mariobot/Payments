using MediatR;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public record IncrementReportRunCountCommand(int ReportId) : IRequest<bool>;
}
