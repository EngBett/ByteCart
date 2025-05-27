using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Suppliers.Commands.DeleteSupplier;

public record DeleteSupplierCommand : IRequest<string>
{
    public string Id { get; set; } = null!;
}

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, string>
{
    private readonly IApplicationDbContext _context;

    public DeleteSupplierCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier == null)
            throw new NotFoundException(nameof(Supplier), request.Id);

        if (supplier.Products.Any())
            throw new ValidationException("Cannot delete supplier with associated products.");

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
