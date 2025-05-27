using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand : IRequest<bool>
{
    public string Id { get; set; } = null!;
}

public class DeleteProductCommandCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
