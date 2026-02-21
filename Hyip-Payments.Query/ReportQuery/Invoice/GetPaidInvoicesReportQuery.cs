using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Invoice
{
    public class GetPaidInvoicesReportQuery : IRequest<PaidInvoicesReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetPaidInvoicesReportQueryHandler : IRequestHandler<GetPaidInvoicesReportQuery, PaidInvoicesReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetPaidInvoicesReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaidInvoicesReportModel> Handle(GetPaidInvoicesReportQuery request, CancellationToken cancellationToken)
        {
            // Get all paid invoices in date range
            var paidInvoices = await _context.Invoices
                .Where(i => i.StatusInvoice == "Paid" 
                         && i.InvoiceDate >= request.StartDate 
                         && i.InvoiceDate <= request.EndDate)
                .Include(i => i.Items)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.Description,
                    ProcessedBy = i.CreatedByUserId,
                    ItemCount = i.Items != null ? i.Items.Count : 0
                })
                .ToListAsync(cancellationToken);

            // Create DTOs with payment timing calculations
            var paidInvoiceDtos = paidInvoices.Select(i =>
            {
                // Simulated payment date
                var paidDate = i.InvoiceDate.AddDays(new Random().Next(1, 30));
                var daysToPay = (paidDate.Date - i.InvoiceDate.Date).Days;

                return new PaidInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber ?? "N/A",
                    InvoiceDate = i.InvoiceDate,
                    PaidDate = paidDate,
                    DaysToPay = daysToPay,
                    TotalAmount = i.TotalAmount,
                    Description = i.Description,
                    PaymentMethod = "Credit Card", // Simulated
                    ItemCount = i.ItemCount,
                    ProcessedBy = i.ProcessedBy ?? "Unknown"
                };
            }).OrderByDescending(i => i.PaidDate ?? i.InvoiceDate).ToList();

            var totalAmount = paidInvoiceDtos.Sum(i => i.TotalAmount);

            // Create summary
            var summary = new PaidSummaryDto
            {
                PaidWithin7Days = paidInvoiceDtos.Count(i => i.DaysToPay <= 7),
                Paid8To30Days = paidInvoiceDtos.Count(i => i.DaysToPay > 7 && i.DaysToPay <= 30),
                PaidOver30Days = paidInvoiceDtos.Count(i => i.DaysToPay > 30),
                PaidWithin7DaysAmount = paidInvoiceDtos.Where(i => i.DaysToPay <= 7).Sum(i => i.TotalAmount),
                Paid8To30DaysAmount = paidInvoiceDtos.Where(i => i.DaysToPay > 7 && i.DaysToPay <= 30).Sum(i => i.TotalAmount),
                PaidOver30DaysAmount = paidInvoiceDtos.Where(i => i.DaysToPay > 30).Sum(i => i.TotalAmount),
                AverageDaysToPay = paidInvoiceDtos.Any() ? (decimal)paidInvoiceDtos.Average(i => i.DaysToPay) : 0,
                LargestPayment = paidInvoiceDtos.Any() ? paidInvoiceDtos.Max(i => i.TotalAmount) : 0,
                SmallestPayment = paidInvoiceDtos.Any() ? paidInvoiceDtos.Min(i => i.TotalAmount) : 0
            };

            return new PaidInvoicesReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPaidInvoices = paidInvoiceDtos.Count,
                TotalPaidAmount = totalAmount,
                AveragePaidAmount = paidInvoiceDtos.Any() ? totalAmount / paidInvoiceDtos.Count : 0,
                PaidInvoices = paidInvoiceDtos,
                Summary = summary
            };
        }
    }
}
