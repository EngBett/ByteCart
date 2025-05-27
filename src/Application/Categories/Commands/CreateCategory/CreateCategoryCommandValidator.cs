using ByteCart.Application.Categories.Commands.CreateCategory;
using ByteCart.Application.Common.Interfaces;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.ParentCategoryId)
            .MustAsync(async (parentId, cancellation) =>
                string.IsNullOrEmpty(parentId) || await dbContext.Categories.AnyAsync(c => c.Id == parentId, cancellation))
            .WithMessage("Parent category does not exist");
    }
}
