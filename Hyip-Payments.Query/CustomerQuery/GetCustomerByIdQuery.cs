using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Query to get a customer by ID
/// </summary>
public class GetCustomerByIdQuery : IRequest<CustomerModel?>
{
    public int Id { get; set; }

    /// <summary>
    /// Include related invoices
    /// </summary>
    public bool IncludeInvoices { get; set; } = false;

    /// <summary>
    /// Include country details
    /// </summary>
    public bool IncludeCountry { get; set; } = true;
}
