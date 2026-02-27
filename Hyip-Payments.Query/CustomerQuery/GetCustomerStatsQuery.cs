using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Query to get customer statistics
/// </summary>
public class GetCustomerStatsQuery : IRequest<CustomerStatsDto>
{
    public int CustomerId { get; set; }
}

/// <summary>
/// DTO for customer statistics
/// </summary>
public class CustomerStatsDto
{
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal AverageInvoiceAmount { get; set; }
    public DateTime? LastInvoiceDate { get; set; }
    public DateTime? LastPaymentDate { get; set; }
}
