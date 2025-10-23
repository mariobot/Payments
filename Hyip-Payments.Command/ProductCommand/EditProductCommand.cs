using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.ProductCommand
{
    // MediatR command to edit a product
    public class EditProductCommand : IRequest<ProductModel>
    {
        public ProductModel Product { get; }
        public EditProductCommand(ProductModel product) => Product = product;
    }

    public class EditProductCommandHandler : IRequestHandler<EditProductCommand, ProductModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public EditProductCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductModel> Handle(EditProductCommand request, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Products.FindAsync(new object[] { request.Product.Id }, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Product with Id {request.Product.Id} not found.");

            // Update properties
            existing.Name = request.Product.Name;
            existing.Description = request.Product.Description;
            existing.Price = request.Product.Price;
            existing.CategoryId = request.Product.CategoryId;
            existing.BrandId = request.Product.BrandId;
            existing.IsActive = request.Product.IsActive;
            existing.CreatedAt = request.Product.CreatedAt;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
