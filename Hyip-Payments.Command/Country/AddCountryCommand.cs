using MediatR;

namespace Hyip_Payments.Command.Country;

public class AddCountryCommand : IRequest<bool>
{
    public string CountryName { get; set; }

    public AddCountryCommand(string countryName)
    {
        CountryName = countryName;
    }
}
