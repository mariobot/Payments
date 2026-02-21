using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Payment
{
    public class GetTransactionLogReportQuery : IRequest<TransactionLogReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class GetTransactionLogReportQueryHandler : IRequestHandler<GetTransactionLogReportQuery, TransactionLogReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetTransactionLogReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionLogReportModel> Handle(GetTransactionLogReportQuery request, CancellationToken cancellationToken)
        {
            // Base query for transactions in date range
            var query = _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate)
                .Include(p => p.PaymentMethod)
                .Include(p => p.Invoice)
                .AsQueryable();

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(request.Status))
            {
                query = query.Where(p => p.Status == request.Status);
            }

            // Apply payment method filter if provided
            if (!string.IsNullOrEmpty(request.PaymentMethod))
            {
                query = query.Where(p => p.PaymentMethod != null && p.PaymentMethod.Name == request.PaymentMethod);
            }

            // Apply search term if provided (search in reference, description, invoice number)
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(p => 
                    (p.Reference != null && p.Reference.ToLower().Contains(searchLower)) ||
                    (p.Description != null && p.Description.ToLower().Contains(searchLower)) ||
                    (p.Invoice != null && p.Invoice.InvoiceNumber != null && p.Invoice.InvoiceNumber.ToLower().Contains(searchLower))
                );
            }

            var transactions = await query
                .OrderByDescending(p => p.TransactionDate)
                .ToListAsync(cancellationToken);

            // Create transaction DTOs
            var transactionDtos = transactions.Select(t => new TransactionLogDto
            {
                TransactionId = t.Id,
                TransactionReference = t.Reference ?? "N/A",
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                Status = t.Status ?? "Unknown",
                PaymentMethod = t.PaymentMethod?.Name ?? "Unknown",
                InvoiceNumber = t.Invoice?.InvoiceNumber,
                InvoiceId = t.InvoiceId,
                Description = t.Description,
                ProcessedBy = t.ProcessedByUserId,
                Notes = t.Description, // Using Description as Notes
                IsActive = true // PaymentTransactions don't have IsActive, assume true
            }).ToList();

            var totalAmount = transactionDtos.Sum(t => t.Amount);

            // Get all transactions for summary (without filters)
            var allTransactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate)
                .Include(p => p.PaymentMethod)
                .ToListAsync(cancellationToken);

            // Create summary
            var summary = new TransactionLogSummaryDto
            {
                CompletedTransactions = allTransactions.Count(t => t.Status == "Completed"),
                PendingTransactions = allTransactions.Count(t => t.Status == "Pending"),
                FailedTransactions = allTransactions.Count(t => t.Status == "Failed"),
                CancelledTransactions = allTransactions.Count(t => t.Status == "Cancelled"),
                CompletedAmount = allTransactions.Where(t => t.Status == "Completed").Sum(t => t.Amount),
                PendingAmount = allTransactions.Where(t => t.Status == "Pending").Sum(t => t.Amount),
                FailedAmount = allTransactions.Where(t => t.Status == "Failed").Sum(t => t.Amount),
                AverageTransactionAmount = allTransactions.Any() ? allTransactions.Average(t => t.Amount) : 0,
                
                // Transactions by payment method
                TransactionsByMethod = allTransactions
                    .GroupBy(t => t.PaymentMethod?.Name ?? "Unknown")
                    .ToDictionary(g => g.Key, g => g.Count()),

                // Transactions by hour of day
                TransactionsByHour = allTransactions
                    .GroupBy(t => t.TransactionDate.Hour)
                    .ToDictionary(g => $"{g.Key:D2}:00", g => g.Count())
            };

            return new TransactionLogReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalTransactions = transactionDtos.Count,
                TotalAmount = totalAmount,
                Transactions = transactionDtos,
                Summary = summary
            };
        }
    }
}
