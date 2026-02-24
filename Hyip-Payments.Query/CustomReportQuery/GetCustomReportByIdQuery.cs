using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomReportQuery
{
    public record GetCustomReportByIdQuery(int Id) : IRequest<CustomReportModel?>;
}
