using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Handler for AddCustomerCommand
/// </summary>
public class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, int>
{
    private readonly PaymentsDbContext _context;

    public AddCustomerCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
    {
        // Generate unique customer number
        var lastCustomer = await _context.Customers
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = (lastCustomer?.Id ?? 0) + 1;
        var customerNumber = $"CUST-{nextNumber:D4}"; // CUST-0001, CUST-0002, etc.

        var customer = new CustomerModel
        {
            CustomerNumber = customerNumber,
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            CountryId = request.CountryId,
            TaxId = request.TaxId,
            PaymentTermsDays = request.PaymentTermsDays,
            CreditLimit = request.CreditLimit,
            DiscountPercentage = request.DiscountPercentage,
            CurrentBalance = 0, // Initial balance is 0
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = request.CreatedByUserId,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
