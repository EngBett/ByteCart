using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await dbContext.Categories.AnyAsync(c => c.Id == id, cancellation))
            .WithMessage("Category does not exist");
    }
}
