using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CategoryCommand
{
    // MediatR command to edit a category
    public class EditCategoryCommand : IRequest<CategoryModel>
    {
        public CategoryModel Category { get; }
        public EditCategoryCommand(CategoryModel category) => Category = category;
    }

    public class EditCategoryCommandHandler : IRequestHandler<EditCategoryCommand, CategoryModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public EditCategoryCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CategoryModel> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Categories.FindAsync(new object[] { request.Category.Id }, cancellationToken);
            if (existing == null)
                throw new KeyNotFoundException($"Category with Id {request.Category.Id} not found.");

            // Update properties
            existing.Name = request.Category.Name;
            existing.Description = request.Category.Description;
            existing.IsActive = request.Category.IsActive;
            existing.CreatedAt = request.Category.CreatedAt;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
