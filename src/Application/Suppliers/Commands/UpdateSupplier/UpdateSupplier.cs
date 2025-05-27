using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace ByteCart.Application.Suppliers.Commands.UpdateSupplier;

public record UpdateSupplierCommand : IRequest<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    public string? Website { get; set; }
}

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, string>
{
    private readonly IApplicationDbContext _context;

    public UpdateSupplierCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers.FindAsync([request.Id], cancellationToken);
        if (supplier == null)
            throw new NotFoundException(nameof(Supplier), request.Id);

        if (!string.IsNullOrEmpty(request.ContactEmail) && request.ContactEmail != supplier.ContactEmail)
        {
            var emailExists = await _context.Suppliers
                .AnyAsync(s => s.ContactEmail == request.ContactEmail && s.Id != request.Id, cancellationToken);
            
            if (emailExists)
                throw new ValidationException("Email already exists.");
        }
        
        supplier.Name = request.Name;
        supplier.ContactEmail = request.ContactEmail;
        supplier.ContactNumber = request.ContactNumber;
        supplier.Website = request.Website;
        supplier.LastModified = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return supplier.Id;
    }
}
