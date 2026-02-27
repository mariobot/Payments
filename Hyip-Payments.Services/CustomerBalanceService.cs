using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hyip_Payments.Services;

/// <summary>
/// Service to automatically update customer balances
/// This service should be called whenever invoices or payments change
/// </summary>
public interface ICustomerBalanceService
{
    Task UpdateCustomerBalanceAsync(int customerId, CancellationToken cancellationToken = default);
    Task UpdateMultipleCustomerBalancesAsync(IEnumerable<int> customerIds, CancellationToken cancellationToken = default);
    Task RecalculateAllCustomerBalancesAsync(CancellationToken cancellationToken = default);
}

public class CustomerBalanceService : ICustomerBalanceService
{
    private readonly PaymentsDbContext _context;
    private readonly ILogger<CustomerBalanceService> _logger;

    public CustomerBalanceService(PaymentsDbContext context, ILogger<CustomerBalanceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Update balance for a single customer
    /// Calculates from unpaid/partially paid invoices
    /// </summary>
    public async Task UpdateCustomerBalanceAsync(int customerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning("Customer {CustomerId} not found for balance update", customerId);
                return;
            }

            // Calculate balance from invoices that are not fully paid
            // StatusInvoice can be: "Draft", "Sent", "Paid", "Overdue", "Cancelled"
            var unpaidInvoices = await _context.Invoices
                .Where(i => i.CustomerId == customerId)
                .Where(i => i.StatusInvoice != "Paid" && i.StatusInvoice != "Cancelled")
                .Where(i => i.IsActive) // Only count active invoices
                .ToListAsync(cancellationToken);

            var oldBalance = customer.CurrentBalance;
            customer.CurrentBalance = unpaidInvoices.Sum(i => i.TotalAmount);
            customer.UpdatedAt = DateTime.UtcNow;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Updated balance for Customer {CustomerId} from {OldBalance} to {NewBalance}",
                customerId, oldBalance, customer.CurrentBalance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating balance for Customer {CustomerId}", customerId);
            throw;
        }
    }

    /// <summary>
    /// Update balances for multiple customers
    /// Useful when bulk operations affect multiple customers
    /// </summary>
    public async Task UpdateMultipleCustomerBalancesAsync(IEnumerable<int> customerIds, CancellationToken cancellationToken = default)
    {
        foreach (var customerId in customerIds.Distinct())
        {
            await UpdateCustomerBalanceAsync(customerId, cancellationToken);
        }
    }

    /// <summary>
    /// Recalculate all customer balances
    /// Use for data reconciliation or maintenance
    /// </summary>
    public async Task RecalculateAllCustomerBalancesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting recalculation of all customer balances");

        var customerIds = await _context.Customers
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var updatedCount = 0;

        foreach (var customerId in customerIds)
        {
            await UpdateCustomerBalanceAsync(customerId, cancellationToken);
            updatedCount++;
        }

        _logger.LogInformation("Completed recalculation of {Count} customer balances", updatedCount);
    }
}
