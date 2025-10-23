using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ProductQuery
{
    // MediatR query to get a product by Id
    public class GetProductByIdQuery : IRequest<ProductModel?>
    {
        public int Id { get; }
        public GetProductByIdQuery(int id) => Id = id;
    }

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetProductByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductModel?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        }
    }
}
