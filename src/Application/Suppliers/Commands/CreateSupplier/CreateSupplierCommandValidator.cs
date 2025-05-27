using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .MustAsync(async (email, cancellation) =>
                !await context.Suppliers.AnyAsync(s => s.ContactEmail == email, cancellation))
            .WithMessage("This contact email is already in use.");
        
        RuleFor(x => x.ContactNumber)
            .NotEmpty()
            .When(x => !string.IsNullOrEmpty(x.ContactNumber))
            .MustAsync(async (number, cancellation) =>
                !await context.Suppliers.AnyAsync(s => s.ContactNumber == number, cancellation))
            .WithMessage("This contact number is already in use.");

        RuleFor(x => x.Website)
            .MaximumLength(255);
    }
}
