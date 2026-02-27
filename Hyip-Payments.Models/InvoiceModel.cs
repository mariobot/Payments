using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("Invoice")]
    public class InvoiceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        [MaxLength(256)]
        public string? Description { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        // New property to track active status
        [Required]
        public bool IsActive { get; set; } = true;

        // Renamed from Status to avoid SQL reserved word conflict
        [Column("StatusInvoice")]
        [MaxLength(32)]
        public string StatusInvoice { get; set; } = "Draft";

        // Customer who this invoice belongs to
        public int? CustomerId { get; set; }

        // User who created/owns this invoice
        [MaxLength(450)] // Standard ASP.NET Identity user ID length
        public string? CreatedByUserId { get; set; }

        // Navigation Properties

        /// <summary>
        /// Navigation to Customer
        /// </summary>
        public CustomerModel? Customer { get; set; }

        // Navigation property for invoice items
        public List<InvoiceItemModel> Items { get; set; } = new();
    }
}
