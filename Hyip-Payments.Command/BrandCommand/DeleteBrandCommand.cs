using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.BrandCommand
{
    // MediatR command to delete a brand by Id
    public class DeleteBrandCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeleteBrandCommand(int id) => Id = id;
    }

    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, bool>
    {
        private readonly PaymentsDbContext _dbContext;

        public DeleteBrandCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await _dbContext.Brands.FindAsync(new object[] { request.Id }, cancellationToken);
            if (brand == null)
                return false;

            _dbContext.Brands.Remove(brand);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
