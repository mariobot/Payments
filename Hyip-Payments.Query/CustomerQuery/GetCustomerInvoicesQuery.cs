using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Query to get all invoices for a specific customer
/// </summary>
public class GetCustomerInvoicesQuery : IRequest<List<InvoiceModel>>
{
    public int CustomerId { get; set; }

    /// <summary>
    /// Filter by invoice status (null = all statuses)
    /// </summary>
    public string? StatusFilter { get; set; }

    /// <summary>
    /// Start date filter (null = no filter)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date filter (null = no filter)
    /// </summary>
    public DateTime? EndDate { get; set; }
}
