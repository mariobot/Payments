using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("Wallet")]
    public class WalletModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(32)]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public decimal Balance { get; set; }

        // Example: Relate to User (if you have a UserModel)
        public string? UserId { get; set; }
        // [ForeignKey(nameof(UserId))]
        // public virtual ApplicationUser? User { get; set; }

        // Example: Relate to Invoices (one wallet can be used in many invoices)
        public virtual List<InvoiceModel> Invoices { get; set; } = new();

        // Example: Track creation date
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Example: Track if wallet is active
        public bool IsActive { get; set; } = true;
    }
}
