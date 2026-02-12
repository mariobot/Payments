using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("InvoiceItem")]
    public class InvoiceItemModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public InvoiceModel Invoice { get; set; } = null!;

        // Optional reference to Product (nullable for custom items)
        public int? ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public ProductModel? Product { get; set; }

        [Required]
        [MaxLength(128)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal Total => Quantity * UnitPrice;
    }
}
