using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Hyip_Payments.Query.ReportQuery.Invoice
{
    public class GetInvoiceSummaryReportQuery : IRequest<InvoiceSummaryReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetInvoiceSummaryReportQueryHandler : IRequestHandler<GetInvoiceSummaryReportQuery, InvoiceSummaryReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetInvoiceSummaryReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceSummaryReportModel> Handle(GetInvoiceSummaryReportQuery request, CancellationToken cancellationToken)
        {
            // Get all invoices in date range
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= request.StartDate 
                         && i.InvoiceDate <= request.EndDate)
                .ToListAsync(cancellationToken);

            // Invoices by status
            var invoicesByStatus = invoices
                .GroupBy(i => i.StatusInvoice ?? "Unknown")
                .Select(g => new InvoiceByStatusDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(i => i.TotalAmount)
                })
                .ToList();

            var totalInvoices = invoices.Count;
            var totalValue = invoices.Sum(i => i.TotalAmount);

            // Calculate percentages
            foreach (var status in invoicesByStatus)
            {
                status.Percentage = totalInvoices > 0 
                    ? (decimal)status.Count / totalInvoices * 100 
                    : 0;
            }

            // Invoices by month
            var invoicesByMonth = invoices
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .Select(g => new InvoiceByMonthDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    InvoiceCount = g.Count(),
                    TotalAmount = g.Sum(i => i.TotalAmount)
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            // Status summary
            var statusSummary = new InvoiceStatusSummaryDto
            {
                DraftCount = invoices.Count(i => i.StatusInvoice == "Draft"),
                PendingCount = invoices.Count(i => i.StatusInvoice == "Pending"),
                PaidCount = invoices.Count(i => i.StatusInvoice == "Paid"),
                CancelledCount = invoices.Count(i => i.StatusInvoice == "Cancelled"),
                DraftAmount = invoices.Where(i => i.StatusInvoice == "Draft").Sum(i => i.TotalAmount),
                PendingAmount = invoices.Where(i => i.StatusInvoice == "Pending").Sum(i => i.TotalAmount),
                PaidAmount = invoices.Where(i => i.StatusInvoice == "Paid").Sum(i => i.TotalAmount),
                CancelledAmount = invoices.Where(i => i.StatusInvoice == "Cancelled").Sum(i => i.TotalAmount)
            };

            return new InvoiceSummaryReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalInvoices = totalInvoices,
                TotalInvoiceValue = totalValue,
                AverageInvoiceAmount = totalInvoices > 0 ? totalValue / totalInvoices : 0,
                InvoicesByStatus = invoicesByStatus,
                InvoicesByMonth = invoicesByMonth,
                StatusSummary = statusSummary
            };
        }
    }
}
