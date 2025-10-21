using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CountryCommand
{
    // MediatR command to edit a country
    public class EditCountryCommand : IRequest<CountryModel?>
    {
        public CountryModel Country { get; }
        public EditCountryCommand(CountryModel country) => Country = country;
    }

    // Handler
    public class EditCountryCommandHandler : IRequestHandler<EditCountryCommand, CountryModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditCountryCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<CountryModel?> Handle(EditCountryCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.Countries.FirstOrDefaultAsync(c => c.Id == request.Country.Id, cancellationToken);
            if (existing == null)
                return null;

            existing.Name = request.Country.Name;
            existing.IsoCode = request.Country.IsoCode;
            existing.Capital = request.Country.Capital;
            existing.Region = request.Country.Region;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
