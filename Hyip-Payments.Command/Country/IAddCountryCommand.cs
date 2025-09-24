using MediatR;

namespace Hyip_Payments.Command.Country
{
    public interface IAddCountryCommand : IRequest<bool>
    {
        string CountryName { get; set; }
    }
}
