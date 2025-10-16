using MediatR;
using Hyip_Payments.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Command.Country
{
    // Interface for AddCountryCommand
    public interface IAddCountryCommand : IRequest<CountryModel>
    {
        CountryModel Country { get; }
    }

    // Implementation of AddCountryCommand
    public class AddCountryCommand : IAddCountryCommand
    {
        public CountryModel Country { get; }
        public string CountryName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AddCountryCommand(CountryModel country)
        {
            Country = country;
        }
    }

    // Handler for AddCountryCommand
    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, CountryModel>
    {
        // Inject your data context or repository here as needed
        // private readonly PaymentsDbContext _context;

        // public AddCountryCommandHandler(PaymentsDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<CountryModel> Handle(AddCountryCommand request, CancellationToken cancellationToken)
        {
            // TODO: Add your logic to persist the country
            // Example:
            // _context.Countries.Add(request.Country);
            // await _context.SaveChangesAsync(cancellationToken);
            // return request.Country;

            // For demonstration, just return the country
            return await Task.FromResult(request.Country);
        }
    }
}