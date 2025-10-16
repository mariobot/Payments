using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("Payments")]
    public class PaymentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(256)]
        public string? Description { get; set; }

        [MaxLength(32)]
        public string? Status { get; set; }

        [MaxLength(128)]
        public string? Payer { get; set; }

        [MaxLength(128)]
        public string? Payee { get; set; }

        // Relationship with Invoice
        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public InvoiceModel Invoice { get; set; } = null!;
    }
}
