using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public record UpdateCustomReportCommand(CustomReportModel Report) : IRequest<CustomReportModel>;
}
