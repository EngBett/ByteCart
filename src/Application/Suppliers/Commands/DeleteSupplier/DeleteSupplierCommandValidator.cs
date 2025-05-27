using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommandValidator : AbstractValidator<DeleteSupplierCommand>
{
    public DeleteSupplierCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await context.Suppliers.AnyAsync(s => s.Id == id, cancellation))
            .WithMessage("Supplier not found.");
    }
}
