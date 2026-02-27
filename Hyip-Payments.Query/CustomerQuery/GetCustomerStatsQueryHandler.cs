using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Handler for GetCustomerStatsQuery
/// </summary>
public class GetCustomerStatsQueryHandler : IRequestHandler<GetCustomerStatsQuery, CustomerStatsDto>
{
    private readonly PaymentsDbContext _context;

    public GetCustomerStatsQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerStatsDto> Handle(GetCustomerStatsQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _context.Invoices
            .Where(i => i.CustomerId == request.CustomerId)
            .ToListAsync(cancellationToken);

        var paidInvoices = invoices.Where(i => i.StatusInvoice == "Paid").ToList();
        var pendingInvoices = invoices.Where(i => i.StatusInvoice == "Pending").ToList();
        var overdueInvoices = invoices.Where(i =>
            i.StatusInvoice == "Pending" &&
            i.InvoiceDate.AddDays(30) < DateTime.UtcNow).ToList();

        var paymentTransactions = await _context.PaymentTransactions
            .Where(pt => invoices.Select(i => i.Id).Contains(pt.InvoiceId ?? 0))
            .ToListAsync(cancellationToken);

        return new CustomerStatsDto
        {
            TotalInvoices = invoices.Count,
            PaidInvoices = paidInvoices.Count,
            PendingInvoices = pendingInvoices.Count,
            OverdueInvoices = overdueInvoices.Count,
            TotalRevenue = paidInvoices.Sum(i => i.TotalAmount),
            OutstandingBalance = pendingInvoices.Sum(i => i.TotalAmount),
            AverageInvoiceAmount = invoices.Any() ? invoices.Average(i => i.TotalAmount) : 0,
            LastInvoiceDate = invoices.Any() ? invoices.Max(i => i.InvoiceDate) : null,
            LastPaymentDate = paymentTransactions.Any() ? paymentTransactions.Max(pt => pt.TransactionDate) : null
        };
    }
}
