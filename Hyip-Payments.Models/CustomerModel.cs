namespace Hyip_Payments.Models;

/// <summary>
/// Represents a customer in the HYIP Payments system
/// </summary>
public class CustomerModel
{
    /// <summary>
    /// Primary key
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique customer number (e.g., CUST-0001, CUST-0002)
    /// </summary>
    public string CustomerNumber { get; set; } = string.Empty;

    /// <summary>
    /// Company or business name
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Contact person name
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Primary email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Physical address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// City
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// State or Province
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Postal or Zip code
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Country ID (foreign key to CountryModel)
    /// </summary>
    public int? CountryId { get; set; }

    /// <summary>
    /// Tax ID or VAT number
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// Payment terms in days (e.g., Net 30, Net 60)
    /// </summary>
    public int PaymentTermsDays { get; set; } = 30;

    /// <summary>
    /// Credit limit for this customer
    /// </summary>
    public decimal? CreditLimit { get; set; }

    /// <summary>
    /// Current outstanding balance (how much customer owes)
    /// </summary>
    public decimal CurrentBalance { get; set; }

    /// <summary>
    /// Customer discount percentage (e.g., 0.05 for 5%)
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Whether the customer is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Additional notes about the customer
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date when customer was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when customer was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User ID who created this customer
    /// </summary>
    public string? CreatedByUserId { get; set; }

    /// <summary>
    /// User ID who last updated this customer
    /// </summary>
    public string? UpdatedByUserId { get; set; }

    // Navigation Properties

    /// <summary>
    /// Navigation to Country
    /// </summary>
    public CountryModel? Country { get; set; }

    /// <summary>
    /// Collection of invoices for this customer
    /// </summary>
    public ICollection<InvoiceModel> Invoices { get; set; } = new List<InvoiceModel>();
}
