using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Invoice
{
    public class GetInvoiceAgingReportQuery : IRequest<InvoiceAgingReportModel>
    {
        public DateTime ReportDate { get; set; } = DateTime.Today;
    }

    public class GetInvoiceAgingReportQueryHandler : IRequestHandler<GetInvoiceAgingReportQuery, InvoiceAgingReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetInvoiceAgingReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceAgingReportModel> Handle(GetInvoiceAgingReportQuery request, CancellationToken cancellationToken)
        {
            // Get all unpaid invoices (Draft and Pending only)
            var unpaidInvoices = await _context.Invoices
                .Where(i => i.StatusInvoice == "Draft" || i.StatusInvoice == "Pending")
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.StatusInvoice
                })
                .ToListAsync(cancellationToken);

            // Calculate aging for each invoice
            var agingDetails = unpaidInvoices.Select(i =>
            {
                var daysOld = (request.ReportDate - i.InvoiceDate.Date).Days;
                var bucket = GetAgingBucket(daysOld);
                var riskLevel = GetRiskLevel(daysOld);

                return new InvoiceAgingDetailDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber ?? "N/A",
                    InvoiceDate = i.InvoiceDate,
                    DaysOld = daysOld,
                    Amount = i.TotalAmount,
                    Status = i.StatusInvoice ?? "Unknown",
                    AgingBucket = bucket,
                    RiskLevel = riskLevel
                };
            }).OrderByDescending(i => i.DaysOld).ToList();

            // Create aging buckets
            var agingBuckets = new List<InvoiceAgingBucketDto>
            {
                new InvoiceAgingBucketDto
                {
                    BucketName = "Current (0-30 days)",
                    MinDays = 0,
                    MaxDays = 30,
                    RiskLevel = "Low"
                },
                new InvoiceAgingBucketDto
                {
                    BucketName = "31-60 days",
                    MinDays = 31,
                    MaxDays = 60,
                    RiskLevel = "Medium"
                },
                new InvoiceAgingBucketDto
                {
                    BucketName = "61-90 days",
                    MinDays = 61,
                    MaxDays = 90,
                    RiskLevel = "High"
                },
                new InvoiceAgingBucketDto
                {
                    BucketName = "Over 90 days",
                    MinDays = 91,
                    MaxDays = null,
                    RiskLevel = "Critical"
                }
            };

            // Populate bucket data
            var totalAmount = agingDetails.Sum(i => i.Amount);
            foreach (var bucket in agingBuckets)
            {
                var bucketInvoices = agingDetails.Where(i => 
                    i.DaysOld >= bucket.MinDays && 
                    (bucket.MaxDays == null || i.DaysOld <= bucket.MaxDays)).ToList();

                bucket.InvoiceCount = bucketInvoices.Count;
                bucket.TotalAmount = bucketInvoices.Sum(i => i.Amount);
                bucket.Percentage = totalAmount > 0 ? (bucket.TotalAmount / totalAmount * 100) : 0;
            }

            // Create summary
            var summary = new AgingSummaryDto
            {
                Current0To30Count = agingBuckets[0].InvoiceCount,
                Days31To60Count = agingBuckets[1].InvoiceCount,
                Days61To90Count = agingBuckets[2].InvoiceCount,
                Over90DaysCount = agingBuckets[3].InvoiceCount,
                Current0To30Amount = agingBuckets[0].TotalAmount,
                Days31To60Amount = agingBuckets[1].TotalAmount,
                Days61To90Amount = agingBuckets[2].TotalAmount,
                Over90DaysAmount = agingBuckets[3].TotalAmount,
                AverageAgeDays = agingDetails.Any() ? Convert.ToDecimal(agingDetails.Average(i => i.DaysOld)) : 0,
                OldestInvoiceDays = agingDetails.Any() ? agingDetails.Max(i => i.DaysOld) : 0
            };

            return new InvoiceAgingReportModel
            {
                ReportDate = request.ReportDate,
                TotalUnpaidInvoices = agingDetails.Count,
                TotalUnpaidAmount = totalAmount,
                AgingBuckets = agingBuckets,
                AgingDetails = agingDetails,
                Summary = summary
            };
        }

        private string GetAgingBucket(int daysOld)
        {
            return daysOld switch
            {
                <= 30 => "Current (0-30 days)",
                <= 60 => "31-60 days",
                <= 90 => "61-90 days",
                _ => "Over 90 days"
            };
        }

        private string GetRiskLevel(int daysOld)
        {
            return daysOld switch
            {
                <= 30 => "Low",
                <= 60 => "Medium",
                <= 90 => "High",
                _ => "Critical"
            };
        }
    }
}
