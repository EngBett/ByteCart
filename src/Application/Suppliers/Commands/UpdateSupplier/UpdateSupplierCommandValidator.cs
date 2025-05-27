using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await context.Suppliers.AnyAsync(s => s.Id == id, cancellation))
            .WithMessage("Supplier not found.");

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail));
        RuleFor(x => x.ContactNumber).NotEmpty();
        RuleFor(x => x.Website).MaximumLength(255);
    }
}
