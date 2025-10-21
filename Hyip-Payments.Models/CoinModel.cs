using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("Coin")]
    public class CoinModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(256)]
        public string? Description { get; set; }

        [Required]
        public decimal CurrentPrice { get; set; }

        [MaxLength(128)]
        public string? Network { get; set; }

        // Add navigation properties or additional fields as needed
    }
}
