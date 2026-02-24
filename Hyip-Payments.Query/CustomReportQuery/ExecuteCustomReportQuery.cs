using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomReportQuery
{
    public record ExecuteCustomReportQuery(
        string DataSource,
        List<string> SelectedColumns,
        DateTime? StartDate,
        DateTime? EndDate,
        bool IncludeCompleted,
        bool IncludePending,
        bool IncludeFailed,
        bool IncludeCancelled,
        string SortBy
    ) : IRequest<List<Dictionary<string, object>>>;
}
