using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CategoryCommand
{
    // MediatR command to delete a category by Id
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeleteCategoryCommand(int id) => Id = id;
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly PaymentsDbContext _dbContext;

        public DeleteCategoryCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _dbContext.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
            if (category == null)
                return false;

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
