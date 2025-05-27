using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await dbContext.Categories.AnyAsync(c => c.Id == id, cancellation))
            .WithMessage("Category does not exist");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.ParentCategoryId)
            .MustAsync(async (command, parentId, cancellation) =>
            {
                if (string.IsNullOrEmpty(parentId))
                    return true;

                // Ensure parent exists and is not the same as current category
                return parentId != command.Id &&
                       await dbContext.Categories.AnyAsync(c => c.Id == parentId, cancellation);
            })
            .WithMessage("Parent category is invalid");
    }
}
