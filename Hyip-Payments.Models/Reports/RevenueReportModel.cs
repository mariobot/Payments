namespace Hyip_Payments.Models.Reports
{
    public class RevenueReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public List<RevenueByDateDto> RevenueByDate { get; set; } = new();
        public List<RevenueByMethodDto> RevenueByPaymentMethod { get; set; } = new();
    }

    public class RevenueByDateDto
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }

    public class RevenueByMethodDto
    {
        public string PaymentMethodName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal Percentage { get; set; }
    }
}
