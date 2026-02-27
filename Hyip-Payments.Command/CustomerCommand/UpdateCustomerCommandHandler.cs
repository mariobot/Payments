using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Handler for UpdateCustomerCommand
/// </summary>
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly PaymentsDbContext _context;

    public UpdateCustomerCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
            return false;

        // Update properties
        customer.CompanyName = request.CompanyName;
        customer.ContactName = request.ContactName;
        customer.Email = request.Email;
        customer.Phone = request.Phone;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.State = request.State;
        customer.PostalCode = request.PostalCode;
        customer.CountryId = request.CountryId;
        customer.TaxId = request.TaxId;
        customer.PaymentTermsDays = request.PaymentTermsDays;
        customer.CreditLimit = request.CreditLimit;
        customer.DiscountPercentage = request.DiscountPercentage;
        customer.Notes = request.Notes;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedByUserId = request.UpdatedByUserId;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
