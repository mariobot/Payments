using MediatR;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Command to add a new customer
/// </summary>
public class AddCustomerCommand : IRequest<int>
{
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public int? CountryId { get; set; }
    public string? TaxId { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public decimal? CreditLimit { get; set; }
    public decimal DiscountPercentage { get; set; }
    public string? Notes { get; set; }
    public string? CreatedByUserId { get; set; }
}
