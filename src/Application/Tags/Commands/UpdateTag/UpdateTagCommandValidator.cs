using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await context.Tags.AnyAsync(t => t.Id == id, cancellation))
            .WithMessage("Tag does not exist.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (command, name, cancellation) =>
                !await context.Tags.AnyAsync(t => t.Name == name && t.Id != command.Id, cancellation))
            .WithMessage("Another tag with this name already exists.");
    }
}
