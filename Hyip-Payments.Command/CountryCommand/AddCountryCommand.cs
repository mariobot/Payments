using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CountryCommand
{
    // Interface for AddCountryCommand
    public interface IAddCountryCommand : IRequest<CountryModel>
    {
        CountryModel Country { get; }
    }

    // Command
    public class AddCountryCommand : IRequest<CountryModel>
    {
        public CountryModel Country { get; }
        public AddCountryCommand(CountryModel country) => Country = country;
    }

    // Handler
    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, CountryModel>
    {
        private readonly PaymentsDbContext _context;
        public AddCountryCommandHandler(PaymentsDbContext context) => _context = context;

        public async Task<CountryModel> Handle(AddCountryCommand request, CancellationToken cancellationToken)
        {
            _context.Countries.Add(request.Country);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Country;
        }
    }

}