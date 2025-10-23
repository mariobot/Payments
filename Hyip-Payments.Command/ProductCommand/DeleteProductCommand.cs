using Hyip_Payments.Context;
using MediatR;

namespace Hyip_Payments.Command.ProductCommand
{
    // MediatR command to delete a product by Id
    public class DeleteProductCommand : IRequest<bool>
    {
        public int Id { get; }
        public DeleteProductCommand(int id) => Id = id;
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly PaymentsDbContext _dbContext;

        public DeleteProductCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _dbContext.Products.FindAsync(new object[] { request.Id }, cancellationToken);
            if (product == null)
                return false;

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
