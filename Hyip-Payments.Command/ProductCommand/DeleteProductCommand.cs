using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.ProductCommand
{
    /// <summary>
    /// Command to disable/deactivate a product (soft delete)
    /// Does not physically delete the product from the database to preserve data integrity
    /// </summary>
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeleteProductCommand(int id) => Id = id;
    }

    /// <summary>
    /// Handler for disabling a product (soft delete)
    /// Sets IsActive = false instead of removing the record from the database
    /// </summary>
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly PaymentsDbContext _dbContext;

        public DeleteProductCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                return false;

            // Soft delete: Set IsActive to false instead of removing from database
            // This preserves data integrity and maintains historical records
            product.IsActive = false;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
