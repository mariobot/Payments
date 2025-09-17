using System;
using System.Globalization;

namespace Hyip_Payments.Shared
{
    public class MoneyModel : IEquatable<MoneyModel>
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public MoneyModel() { }

        public MoneyModel(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1:N2}", Currency, Amount);
        }

        public override bool Equals(object obj) => Equals(obj as MoneyModel);

        public bool Equals(MoneyModel other)
        {
            if (other is null) return false;
            return Amount == other.Amount && string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency?.ToUpperInvariant());
        }
    }
}
