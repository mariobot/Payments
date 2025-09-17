
namespace Hyip_Payments.Models
{
    public class PaymentModel
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Payer { get; set; }
        public string? Payee { get; set; }

        public PaymentModel() { }

        public PaymentModel(Guid id, decimal amount, string currency, DateTime date, string? description = null, string? status = null, string? payer = null, string? payee = null)
        {
            Id = id;
            Amount = amount;
            Currency = currency;
            Date = date;
            Description = description;
            Status = status;
            Payer = payer;
            Payee = payee;
        }
    }
}
