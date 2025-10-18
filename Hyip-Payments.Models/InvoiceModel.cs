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

        // Navigation property for invoice items
        public List<InvoiceItemModel> Items { get; set; } = new();
    }
}
