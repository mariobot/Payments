using Hyip_Payments.Context;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Services;

/// <summary>
/// Service for generating sequential invoice numbers
/// Format: INV-001, INV-002, INV-003, etc.
/// Thread-safe to prevent duplicate numbers
/// </summary>
public class InvoiceNumberService
{
    private readonly PaymentsDbContext _context;
    private static readonly object _lock = new object();

    public InvoiceNumberService(PaymentsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Generate next sequential invoice number
    /// Format: INV-001, INV-002, etc.
    /// Thread-safe with lock to prevent duplicates
    /// </summary>
    public async Task<string> GenerateNextInvoiceNumberAsync()
    {
        lock (_lock)
        {
            // Get the last invoice number
            var lastInvoice = _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith("INV-"))
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                // Extract number from format INV-XXX
                var numberPart = lastInvoice.InvoiceNumber.Replace("INV-", "");
                if (int.TryParse(numberPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            // Format with leading zeros (3 digits)
            return $"INV-{nextNumber:D3}";
        }
    }

    /// <summary>
    /// Generate next invoice number with custom prefix
    /// </summary>
    public async Task<string> GenerateNextInvoiceNumberAsync(string prefix)
    {
        lock (_lock)
        {
            var searchPrefix = $"{prefix}-";
            
            var lastInvoice = _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith(searchPrefix))
                .OrderByDescending(i => i.Id)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
            {
                var numberPart = lastInvoice.InvoiceNumber.Replace(searchPrefix, "");
                if (int.TryParse(numberPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"{prefix}-{nextNumber:D3}";
        }
    }

    /// <summary>
    /// Check if invoice number already exists
    /// </summary>
    public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber)
    {
        return await _context.Invoices
            .AnyAsync(i => i.InvoiceNumber == invoiceNumber);
    }

    /// <summary>
    /// Get next available number (without generating)
    /// </summary>
    public async Task<int> GetNextAvailableNumberAsync()
    {
        var lastInvoice = await _context.Invoices
            .Where(i => i.InvoiceNumber.StartsWith("INV-"))
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();

        if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
        {
            var numberPart = lastInvoice.InvoiceNumber.Replace("INV-", "");
            if (int.TryParse(numberPart, out int currentNumber))
            {
                return currentNumber + 1;
            }
        }

        return 1;
    }

    /// <summary>
    /// Reset numbering (use with caution - for testing only)
    /// </summary>
    public async Task<string> GenerateInvoiceNumberFromStartAsync(int startNumber)
    {
        return $"INV-{startNumber:D3}";
    }
}
