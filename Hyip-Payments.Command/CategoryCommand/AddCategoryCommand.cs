using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CategoryCommand
{
    // MediatR command to add a category
    public class AddCategoryCommand : IRequest<CategoryModel>
    {
        public CategoryModel Category { get; }
        public AddCategoryCommand(CategoryModel category) => Category = category;
    }

    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, CategoryModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public AddCategoryCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoryModel> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Categories.Add(request.Category);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return request.Category;
        }
    }
}
