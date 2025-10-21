using Hyip_Payments.Context;
using MediatR;

namespace Hyip_Payments.Command.CountryCommand
{
    // MediatR command to delete a country by ID
    public class DeleteCountryCommand : IRequest<bool>
    {
        public int CountryId { get; }
        public DeleteCountryCommand(int countryId) => CountryId = countryId;
    }

    // Handler
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteCountryCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _context.Countries.FindAsync(new object[] { request.CountryId }, cancellationToken);
            if (country == null)
                return false;

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
