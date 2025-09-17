namespace Hyip_Payments.Models 
{
    public class ViewMoneyModel
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Country { get; set; } // Added country property
        public bool IsCurrencyValidForCountry()
        {
            // Example: simple validation logic
            if (Country == "USA" && Currency == "USD") return true;
            if (Country == "Japan" && Currency == "JPY") return true;
            // Add more country-currency pairs as needed
            return false;
        }
    }

}


