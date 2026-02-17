using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("PaymentTransaction")]
    public class PaymentTransactionModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Avoid SQL reserved word - use Column attribute
        [Required]
        [Column("StatusTransaction")]
        [MaxLength(32)]
        public string Status { get; set; } = "Pending"; // e.g., Pending, Completed, Failed

        // Foreign key to Wallet
        public int WalletId { get; set; }
        [ForeignKey(nameof(WalletId))]
        public virtual WalletModel Wallet { get; set; } = null!;

        // Foreign key to PaymentMethod
        public int PaymentMethodId { get; set; }
        [ForeignKey(nameof(PaymentMethodId))]
        public virtual PaymentMethodModel PaymentMethod { get; set; } = null!;

        // Foreign key to Invoice (optional, if transaction is linked to an invoice)
        public int? InvoiceId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public virtual InvoiceModel? Invoice { get; set; }

        [MaxLength(128)]
        public string? Reference { get; set; } // External reference or transaction hash

        public string? Description { get; set; }

        // User who processed this payment
        [MaxLength(450)] // Standard ASP.NET Identity user ID length
        public string? ProcessedByUserId { get; set; }
    }
}
