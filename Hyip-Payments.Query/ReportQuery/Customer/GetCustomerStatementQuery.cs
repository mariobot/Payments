using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Customer;

/// <summary>
/// Query to get customer statement for a specific period
/// </summary>
public class GetCustomerStatementQuery : IRequest<CustomerStatementReportModel>
{
    public int CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IncludeDraft { get; set; } = false;
    public bool IncludeCancelled { get; set; } = false;
}

public class GetCustomerStatementQueryHandler : IRequestHandler<GetCustomerStatementQuery, CustomerStatementReportModel>
{
    private readonly PaymentsDbContext _context;

    public GetCustomerStatementQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerStatementReportModel> Handle(GetCustomerStatementQuery request, CancellationToken cancellationToken)
    {
        // 1. Get customer details
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            throw new InvalidOperationException($"Customer with ID {request.CustomerId} not found");
        }

        // 2. Calculate opening balance (all unpaid invoices before period start)
        var openingBalance = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.CustomerId == request.CustomerId)
            .Where(i => i.InvoiceDate < request.StartDate)
            .Where(i => i.StatusInvoice != "Paid" && i.StatusInvoice != "Cancelled")
            .Where(i => i.IsActive)
            .SumAsync(i => i.TotalAmount, cancellationToken);

        // 3. Get all invoices in period
        var invoicesQuery = _context.Invoices
            .AsNoTracking()
            .Where(i => i.CustomerId == request.CustomerId)
            .Where(i => i.InvoiceDate >= request.StartDate && i.InvoiceDate <= request.EndDate)
            .Where(i => i.IsActive);

        if (!request.IncludeDraft)
        {
            invoicesQuery = invoicesQuery.Where(i => i.StatusInvoice != "Draft");
        }

        if (!request.IncludeCancelled)
        {
            invoicesQuery = invoicesQuery.Where(i => i.StatusInvoice != "Cancelled");
        }

        var invoices = await invoicesQuery
            .OrderBy(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);

        // 4. Get all payments in period
        var payments = await _context.PaymentTransactions
            .AsNoTracking()
            .Where(p => p.Invoice != null && p.Invoice.CustomerId == request.CustomerId)
            .Where(p => p.TransactionDate >= request.StartDate && p.TransactionDate <= request.EndDate)
            .Include(p => p.Invoice)
            .OrderBy(p => p.TransactionDate)
            .ToListAsync(cancellationToken);

        // 5. Combine and sort all transactions
        var transactions = new List<CustomerStatementTransactionModel>();

        // Add invoices
        foreach (var invoice in invoices)
        {
            transactions.Add(new CustomerStatementTransactionModel
            {
                Date = invoice.InvoiceDate,
                Type = "Invoice",
                ReferenceNumber = invoice.InvoiceNumber,
                Description = invoice.Description ?? "Invoice",
                InvoiceAmount = invoice.TotalAmount,
                PaymentAmount = null,
                Status = invoice.StatusInvoice,
                RunningBalance = 0 // Will calculate below
            });
        }

        // Add payments
        foreach (var payment in payments)
        {
            transactions.Add(new CustomerStatementTransactionModel
            {
                Date = payment.TransactionDate,
                Type = "Payment",
                ReferenceNumber = $"PAY-{payment.Id}",
                Description = $"Payment for {payment.Invoice?.InvoiceNumber}",
                InvoiceAmount = null,
                PaymentAmount = payment.Amount,
                Status = "Completed",
                RunningBalance = 0 // Will calculate below
            });
        }

        // Sort all transactions by date
        transactions = transactions.OrderBy(t => t.Date).ThenBy(t => t.Type).ToList();

        // 6. Calculate running balance
        decimal runningBalance = openingBalance;
        foreach (var transaction in transactions)
        {
            if (transaction.Type == "Invoice")
            {
                runningBalance += transaction.InvoiceAmount ?? 0;
            }
            else if (transaction.Type == "Payment")
            {
                runningBalance -= transaction.PaymentAmount ?? 0;
            }
            transaction.RunningBalance = runningBalance;
        }

        // 7. Calculate totals
        var totalInvoiced = invoices.Sum(i => i.TotalAmount);
        var totalPaid = payments.Sum(p => p.Amount);
        var closingBalance = runningBalance;

        // 8. Build report
        var report = new CustomerStatementReportModel
        {
            CustomerId = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            CompanyName = customer.CompanyName,
            ContactName = customer.ContactName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address ?? string.Empty,
            StatementDate = DateTime.UtcNow,
            PeriodStart = request.StartDate,
            PeriodEnd = request.EndDate,
            OpeningBalance = openingBalance,
            ClosingBalance = closingBalance,
            TotalInvoiced = totalInvoiced,
            TotalPaid = totalPaid,
            Transactions = transactions
        };

        return report;
    }
}
