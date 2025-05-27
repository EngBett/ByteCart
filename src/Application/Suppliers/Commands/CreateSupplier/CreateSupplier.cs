using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand : IRequest<string>
{
    public string Name { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    public string? Website { get; set; }
}

public class CreateSupplierCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateSupplierCommand, string>
{
    public async Task<string> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        
        var supplier = new Supplier
        {
            Name = request.Name,
            ContactEmail = request.ContactEmail,
            ContactNumber = request.ContactNumber,
            Website = request.Website
        };

        context.Suppliers.Add(supplier);
        await context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
