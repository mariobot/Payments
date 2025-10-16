
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hyip_Payments.Models;

[Table("Money")]
public class MoneyModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(3)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    public bool IsCurrencyValidForCountry()
    {
        // Example: simple validation logic
        if (Country == "USA" && Currency == "USD") return true;
        if (Country == "Japan" && Currency == "JPY") return true;
        // Add more country-currency pairs as needed
        return false;
    }

    public async Task ProcessAsync()
    {
        throw new NotImplementedException();
    }
}

