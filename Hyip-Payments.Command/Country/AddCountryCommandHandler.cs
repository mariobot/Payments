using MediatR;

namespace Hyip_Payments.Command.Country;

public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, bool>
{
    public Task<bool> Handle(AddCountryCommand request, CancellationToken cancellationToken)
    {
        // Implement your logic to add the country here.
        // For demonstration, always return true.
        // Replace with actual persistence logic as needed.
        return Task.FromResult(true);
    }
}
