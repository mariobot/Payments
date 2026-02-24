using MediatR;

namespace Hyip_Payments.Command.CustomReportCommand
{
    public record DeleteCustomReportCommand(int Id) : IRequest<bool>;
}
