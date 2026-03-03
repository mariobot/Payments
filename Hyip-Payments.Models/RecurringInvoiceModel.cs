using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models;

/// <summary>
/// Recurring Invoice Template - For subscription-based billing
/// </summary>
[Table("RecurringInvoice")]
public class RecurringInvoiceModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string TemplateName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    // Foreign key to Customer
    [Required]
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public virtual CustomerModel Customer { get; set; } = null!;

    // Frequency: Monthly, Quarterly, Annually
    [Required]
    [MaxLength(20)]
    public string Frequency { get; set; } = "Monthly";

    // Day of month to generate (1-31)
    [Required]
    [Range(1, 31)]
    public int DayOfMonth { get; set; } = 1;

    // Recurrence period
    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; } // Null = indefinite

    // Last generated invoice date
    public DateTime? LastGeneratedDate { get; set; }

    // Next scheduled generation date
    public DateTime? NextGenerationDate { get; set; }

    // Number of invoices generated from this template
    public int GeneratedInvoiceCount { get; set; } = 0;

    // Status
    public bool IsActive { get; set; } = true;

    // Invoice details
    [MaxLength(500)]
    public string? InvoiceDescription { get; set; }

    // Auto-send invoice after generation
    public bool AutoSendEmail { get; set; } = false;

    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(450)]
    public string? CreatedByUserId { get; set; }

    [MaxLength(450)]
    public string? UpdatedByUserId { get; set; }

    // Navigation property for template items
    public virtual ICollection<RecurringInvoiceItemModel> TemplateItems { get; set; } = new List<RecurringInvoiceItemModel>();
}

/// <summary>
/// Recurring Invoice Item - Template items for recurring invoices
/// </summary>
[Table("RecurringInvoiceItem")]
public class RecurringInvoiceItemModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Foreign key to RecurringInvoice
    [Required]
    public int RecurringInvoiceId { get; set; }
    [ForeignKey(nameof(RecurringInvoiceId))]
    public virtual RecurringInvoiceModel RecurringInvoice { get; set; } = null!;

    // Optional link to Product (if using catalog)
    public int? ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public virtual ProductModel? Product { get; set; }

    [Required]
    [MaxLength(200)]
    public string ItemName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    // Calculated property
    [NotMapped]
    public decimal Total => Quantity * UnitPrice;
}
