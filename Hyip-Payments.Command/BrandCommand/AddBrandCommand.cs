using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.BrandCommand
{
    // MediatR command to add a brand
    public class AddBrandCommand : IRequest<BrandModel>
    {
        public BrandModel Brand { get; }
        public AddBrandCommand(BrandModel brand) => Brand = brand;
    }

    public class AddBrandCommandHandler : IRequestHandler<AddBrandCommand, BrandModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public AddBrandCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BrandModel> Handle(AddBrandCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Brands.Add(request.Brand);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return request.Brand;
        }
    }
}
