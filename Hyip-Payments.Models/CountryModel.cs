using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models
{
    [Table("Country")]
    public class CountryModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(3)]
        public string? IsoCode { get; set; }

        [MaxLength(100)]
        public string? Capital { get; set; }

        [MaxLength(100)]
        public string? Region { get; set; }
    }
}
