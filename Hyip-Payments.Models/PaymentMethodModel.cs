using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("PaymentMethod")]
    public class PaymentMethodModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Description { get; set; }

        // Example: Relate to Wallets (one payment method can have many wallets)
        public virtual List<WalletModel> Wallets { get; set; } = new();

        // Example: Relate to Invoices (if invoices can specify a payment method)
        public virtual List<InvoiceModel> Invoices { get; set; } = new();

        // Track if the payment method is active
        public bool IsActive { get; set; } = true;

        // Track creation date
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
