using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomReportQuery
{
    public record GetSavedReportsQuery(string UserId, bool IncludePublic = true) : IRequest<List<CustomReportModel>>;
}
