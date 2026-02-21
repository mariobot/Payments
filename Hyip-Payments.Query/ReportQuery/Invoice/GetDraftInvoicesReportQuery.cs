using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Invoice
{
    public class GetDraftInvoicesReportQuery : IRequest<DraftInvoicesReportModel>
    {
        public DateTime ReportDate { get; set; } = DateTime.Today;
    }

    public class GetDraftInvoicesReportQueryHandler : IRequestHandler<GetDraftInvoicesReportQuery, DraftInvoicesReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetDraftInvoicesReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<DraftInvoicesReportModel> Handle(GetDraftInvoicesReportQuery request, CancellationToken cancellationToken)
        {
            // Get all draft invoices
            var draftInvoices = await _context.Invoices
                .Where(i => i.StatusInvoice == "Draft")
                .Include(i => i.InvoiceItems)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.InvoiceDate,
                    i.TotalAmount,
                    i.Description,
                    i.IsActive,
                    i.CreatedBy,
                    ItemCount = i.InvoiceItems != null ? i.InvoiceItems.Count : 0
                })
                .ToListAsync(cancellationToken);

            // Calculate days in draft and create DTOs
            var draftInvoiceDtos = draftInvoices.Select(i =>
            {
                var daysInDraft = (request.ReportDate - i.InvoiceDate.Date).Days;
                
                return new DraftInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber ?? "N/A",
                    InvoiceDate = i.InvoiceDate,
                    DaysInDraft = daysInDraft,
                    TotalAmount = i.TotalAmount,
                    Description = i.Description,
                    ItemCount = i.ItemCount,
                    IsActive = i.IsActive,
                    CreatedBy = i.CreatedBy ?? "Unknown"
                };
            }).OrderByDescending(i => i.DaysInDraft).ToList();

            var totalAmount = draftInvoiceDtos.Sum(i => i.TotalAmount);

            // Create summary
            var summary = new DraftSummaryDto
            {
                NewDrafts0To7Days = draftInvoiceDtos.Count(i => i.DaysInDraft <= 7),
                Medium8To30Days = draftInvoiceDtos.Count(i => i.DaysInDraft > 7 && i.DaysInDraft <= 30),
                OldOver30Days = draftInvoiceDtos.Count(i => i.DaysInDraft > 30),
                NewDraftsAmount = draftInvoiceDtos.Where(i => i.DaysInDraft <= 7).Sum(i => i.TotalAmount),
                MediumDraftsAmount = draftInvoiceDtos.Where(i => i.DaysInDraft > 7 && i.DaysInDraft <= 30).Sum(i => i.TotalAmount),
                OldDraftsAmount = draftInvoiceDtos.Where(i => i.DaysInDraft > 30).Sum(i => i.TotalAmount),
                OldestDraftDays = draftInvoiceDtos.Any() ? draftInvoiceDtos.Max(i => i.DaysInDraft) : 0,
                LargestDraftAmount = draftInvoiceDtos.Any() ? draftInvoiceDtos.Max(i => i.TotalAmount) : 0
            };

            return new DraftInvoicesReportModel
            {
                ReportDate = request.ReportDate,
                TotalDraftInvoices = draftInvoiceDtos.Count,
                TotalDraftAmount = totalAmount,
                AverageDraftAmount = draftInvoiceDtos.Any() ? totalAmount / draftInvoiceDtos.Count : 0,
                DraftInvoices = draftInvoiceDtos,
                Summary = summary
            };
        }
    }
}
