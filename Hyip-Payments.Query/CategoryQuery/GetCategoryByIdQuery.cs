using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CategoryQuery
{
    // MediatR query to get a category by Id
    public class GetCategoryByIdQuery : IRequest<CategoryModel?>
    {
        public int Id { get; }
        public GetCategoryByIdQuery(int id) => Id = id;
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetCategoryByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoryModel?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        }
    }
}
