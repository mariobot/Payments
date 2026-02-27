namespace Hyip_Payments.Models.Reports;

/// <summary>
/// Customer Statement Report - Shows all transactions for a customer in a period
/// </summary>
public class CustomerStatementReportModel
{
    public int CustomerId { get; set; }
    public string CustomerNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime StatementDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    public decimal OpeningBalance { get; set; }
    public decimal ClosingBalance { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    
    public List<CustomerStatementTransactionModel> Transactions { get; set; } = new();
}

/// <summary>
/// Individual transaction line in the statement
/// </summary>
public class CustomerStatementTransactionModel
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty; // "Invoice" or "Payment"
    public string ReferenceNumber { get; set; } = string.Empty; // Invoice or Payment number
    public string Description { get; set; } = string.Empty;
    public decimal? InvoiceAmount { get; set; } // Amount charged
    public decimal? PaymentAmount { get; set; } // Amount received
    public decimal RunningBalance { get; set; } // Balance after this transaction
    public string Status { get; set; } = string.Empty; // For invoices: Draft, Sent, Paid, etc.
}

/// <summary>
/// Request parameters for customer statement
/// </summary>
public class CustomerStatementRequest
{
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IncludeDraft { get; set; } = false;
    public bool IncludeCancelled { get; set; } = false;
}
