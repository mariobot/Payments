using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Query to get all customers
/// </summary>
public class GetCustomerListQuery : IRequest<List<CustomerModel>>
{
    /// <summary>
    /// Filter by active status (null = all, true = active only, false = inactive only)
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Search term to filter customers
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Order by field (CustomerNumber, CompanyName, CreatedAt, CurrentBalance)
    /// </summary>
    public string? OrderBy { get; set; } = "CustomerNumber";

    /// <summary>
    /// Order direction (true = ascending, false = descending)
    /// </summary>
    public bool OrderAscending { get; set; } = true;
}
