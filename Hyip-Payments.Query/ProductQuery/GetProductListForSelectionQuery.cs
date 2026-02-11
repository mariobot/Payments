using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ProductQuery
{
    /// <summary>
    /// Query to get simplified product list for selection dropdowns
    /// </summary>
    public class GetProductListForSelectionQuery : IRequest<List<ProductSelectionDto>>
    {
    }

    /// <summary>
    /// DTO for product selection - lightweight for dropdowns
    /// </summary>
    public class ProductSelectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class GetProductListForSelectionQueryHandler : IRequestHandler<GetProductListForSelectionQuery, List<ProductSelectionDto>>
    {
        private readonly PaymentsDbContext _context;

        public GetProductListForSelectionQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductSelectionDto>> Handle(GetProductListForSelectionQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products
                .Where(p => p.IsActive) // Only active products
                .OrderBy(p => p.Name)
                .Select(p => new ProductSelectionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    IsAvailable = p.IsActive // Use IsActive instead of Stock
                })
                .ToListAsync(cancellationToken);
        }
    }
}
