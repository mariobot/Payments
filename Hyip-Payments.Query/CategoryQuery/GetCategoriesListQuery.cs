using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CategoryQuery
{
    // MediatR query to get all categories
    public class GetCategoriesListQuery : IRequest<List<CategoryModel>>
    {
    }

    public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, List<CategoryModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetCategoriesListQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CategoryModel>> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Categories
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
