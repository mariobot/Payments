using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomReportQuery
{
    public class ExecuteCustomReportQueryHandler : IRequestHandler<ExecuteCustomReportQuery, List<Dictionary<string, object>>>
    {
        private readonly PaymentsDbContext _context;

        public ExecuteCustomReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Dictionary<string, object>>> Handle(ExecuteCustomReportQuery request, CancellationToken cancellationToken)
        {
            return request.DataSource switch
            {
                "Invoices" => await GetInvoicesData(request, cancellationToken),
                "Payments" => await GetPaymentsData(request, cancellationToken),
                "Users" => await GetUsersData(request, cancellationToken),
                "Products" => await GetProductsData(request, cancellationToken),
                _ => new List<Dictionary<string, object>>()
            };
        }

        private async Task<List<Dictionary<string, object>>> GetInvoicesData(ExecuteCustomReportQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Invoices.AsQueryable();

            // Apply date filter
            if (request.StartDate.HasValue)
                query = query.Where(i => i.InvoiceDate >= request.StartDate.Value);
            if (request.EndDate.HasValue)
                query = query.Where(i => i.InvoiceDate <= request.EndDate.Value);

            // Apply status filter
            var statusList = new List<string>();
            if (request.IncludeCompleted) statusList.Add("Paid");
            if (request.IncludePending) statusList.Add("Pending");
            if (request.IncludeFailed) statusList.Add("Failed");
            if (request.IncludeCancelled) statusList.Add("Cancelled");

            if (statusList.Any())
                query = query.Where(i => statusList.Contains(i.StatusInvoice));

            // Apply sorting
            query = ApplySorting(query, request.SortBy);

            var invoices = await query.ToListAsync(cancellationToken);

            // Map to dictionary based on selected columns
            return invoices.Select(invoice =>
            {
                var row = new Dictionary<string, object>();
                foreach (var column in request.SelectedColumns)
                {
                    row[column] = column switch
                    {
                        "InvoiceNumber" => (object)invoice.InvoiceNumber,
                        "Date" => invoice.InvoiceDate,
                        "Amount" => invoice.TotalAmount,
                        "Status" => invoice.StatusInvoice ?? "Unknown",
                        "Customer" => invoice.CreatedByUserId ?? "N/A",
                        _ => "N/A"
                    };
                }
                return row;
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetPaymentsData(ExecuteCustomReportQuery request, CancellationToken cancellationToken)
        {
            var query = _context.PaymentTransactions
                .Include(pt => pt.PaymentMethod)
                .AsQueryable();

            // Apply date filter
            if (request.StartDate.HasValue)
                query = query.Where(p => p.TransactionDate >= request.StartDate.Value);
            if (request.EndDate.HasValue)
                query = query.Where(p => p.TransactionDate <= request.EndDate.Value);

            // Apply status filter
            var statusList = new List<string>();
            if (request.IncludeCompleted) statusList.Add("Completed");
            if (request.IncludePending) statusList.Add("Pending");
            if (request.IncludeFailed) statusList.Add("Failed");
            if (request.IncludeCancelled) statusList.Add("Cancelled");

            if (statusList.Any())
                query = query.Where(p => statusList.Contains(p.Status));

            // Apply sorting
            query = ApplySortingPayments(query, request.SortBy);

            var payments = await query.ToListAsync(cancellationToken);

            // Map to dictionary based on selected columns
            return payments.Select(payment =>
            {
                var row = new Dictionary<string, object>();
                foreach (var column in request.SelectedColumns)
                {
                    row[column] = column switch
                    {
                        "Reference" => (object)(payment.Reference ?? $"TXN-{payment.Id}"),
                        "Date" => payment.TransactionDate,
                        "Amount" => payment.Amount,
                        "Status" => payment.Status ?? "Unknown",
                        "Method" => payment.PaymentMethod?.Name ?? "N/A",
                        _ => "N/A"
                    };
                }
                return row;
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetUsersData(ExecuteCustomReportQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsQueryable();

            // Apply date filter (using CreatedAt)
            if (request.StartDate.HasValue)
                query = query.Where(u => u.CreatedAt >= request.StartDate.Value);
            if (request.EndDate.HasValue)
                query = query.Where(u => u.CreatedAt <= request.EndDate.Value);

            // Apply active status filter
            if (!request.IncludeCancelled)
                query = query.Where(u => u.IsActive);

            var users = await query.ToListAsync(cancellationToken);

            // Map to dictionary based on selected columns
            return users.Select(user =>
            {
                var row = new Dictionary<string, object>();
                foreach (var column in request.SelectedColumns)
                {
                    row[column] = column switch
                    {
                        "Username" => (object)user.Username,
                        "Email" => user.Email,
                        "Date" => user.CreatedAt,
                        "Status" => user.IsActive ? "Active" : "Inactive",
                        "Role" => "User", // You can enhance this with actual roles
                        _ => "N/A"
                    };
                }
                return row;
            }).ToList();
        }

        private async Task<List<Dictionary<string, object>>> GetProductsData(ExecuteCustomReportQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            // Apply active status filter
            if (!request.IncludeCancelled)
                query = query.Where(p => p.IsActive);

            var products = await query.ToListAsync(cancellationToken);

            // Map to dictionary based on selected columns
            return products.Select(product =>
            {
                var row = new Dictionary<string, object>();
                foreach (var column in request.SelectedColumns)
                {
                    row[column] = column switch
                    {
                        "Name" => (object)product.Name,
                        "SKU" => $"SKU-{product.Id}", // Generate SKU from ID since there's no SKU property
                        "Price" => product.Price,
                        "Category" => product.Category?.Name ?? "N/A",
                        "Brand" => product.Brand?.Name ?? "N/A",
                        "Stock" => 0, // No stock property, default to 0
                        "Status" => product.IsActive ? "Active" : "Inactive",
                        _ => "N/A"
                    };
                }
                return row;
            }).ToList();
        }

        private IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sortBy) where T : class
        {
            return sortBy switch
            {
                "Date-Desc" => query.OrderByDescending(x => EF.Property<DateTime>(x, "InvoiceDate")),
                "Date-Asc" => query.OrderBy(x => EF.Property<DateTime>(x, "InvoiceDate")),
                "Amount-Desc" => query.OrderByDescending(x => EF.Property<decimal>(x, "TotalAmount")),
                "Amount-Asc" => query.OrderBy(x => EF.Property<decimal>(x, "TotalAmount")),
                _ => query.OrderByDescending(x => EF.Property<DateTime>(x, "InvoiceDate"))
            };
        }

        private IQueryable<T> ApplySortingPayments<T>(IQueryable<T> query, string sortBy) where T : class
        {
            return sortBy switch
            {
                "Date-Desc" => query.OrderByDescending(x => EF.Property<DateTime>(x, "TransactionDate")),
                "Date-Asc" => query.OrderBy(x => EF.Property<DateTime>(x, "TransactionDate")),
                "Amount-Desc" => query.OrderByDescending(x => EF.Property<decimal>(x, "Amount")),
                "Amount-Asc" => query.OrderBy(x => EF.Property<decimal>(x, "Amount")),
                _ => query.OrderByDescending(x => EF.Property<DateTime>(x, "TransactionDate"))
            };
        }
    }
}
