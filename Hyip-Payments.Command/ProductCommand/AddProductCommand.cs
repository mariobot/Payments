using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.ProductCommand
{
    // MediatR command to add a product
    public class AddProductCommand : IRequest<ProductModel>
    {
        public ProductModel Product { get; }
        public AddProductCommand(ProductModel product) => Product = product;
    }

    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, ProductModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public AddProductCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductModel> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Products.Add(request.Product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return request.Product;
        }
    }
}
