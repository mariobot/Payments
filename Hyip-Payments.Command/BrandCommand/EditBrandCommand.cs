using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.BrandCommand
{
    // MediatR command to edit a brand
    public class EditBrandCommand : IRequest<BrandModel>
    {
        public BrandModel Brand { get; }
        public EditBrandCommand(BrandModel brand) => Brand = brand;
    }

    public class EditBrandCommandHandler : IRequestHandler<EditBrandCommand, BrandModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public EditBrandCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BrandModel> Handle(EditBrandCommand request, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Brands.FindAsync(new object[] { request.Brand.Id }, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Brand with Id {request.Brand.Id} not found.");

            // Update properties
            existing.Name = request.Brand.Name;
            existing.Description = request.Brand.Description;
            existing.LogoUrl = request.Brand.LogoUrl;
            existing.IsActive = request.Brand.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
