using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("Payments")]
    public class PaymentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }

        [Required]
        [MaxLength(128)]
        public string SenderUserId { get; set; }

        [Required]
        [MaxLength(128)]
        public string ReceiverUserId { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        [MaxLength(32)]
        public string Status { get; set; } // e.g., Pending, Completed, Failed

        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        [MaxLength(64)]
        public string TransactionId { get; set; }
    }
}
