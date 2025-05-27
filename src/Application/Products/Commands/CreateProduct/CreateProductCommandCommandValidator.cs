using ByteCart.Application.Common.Interfaces;

namespace ByteCart.Application.Products.Commands.CreateProductCommand;

public class CreateProductCommandValidator : AbstractValidator<CreateProduct.CreateProductCommand>
{
    public CreateProductCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .MaximumLength(50)
            .WithMessage("SKU must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.SupplierId)
            .NotEmpty()
            .MustAsync(async (supplierId, cancellation) =>
                await dbContext.Suppliers.AnyAsync(s => s.Id == supplierId, cancellation))
            .WithMessage("Supplier does not exist");

        RuleFor(x => x.LaunchDate)
            .LessThan(x => x.EndDate).When(x => x.LaunchDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Launch date must be earlier than end date");
        
        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("At least one category is required")
            .MustAsync(async (categoryIds, cancellation) =>
                await dbContext.Categories
                    .Where(c => categoryIds!.Contains(c.Id))
                    .CountAsync(cancellation) == categoryIds!.Count)
            .WithMessage("One or more Category IDs are invalid");

        RuleFor(x => x.TagIds)
            .MustAsync(async (tagIds, cancellation) =>
                tagIds == null || !tagIds.Any() ||
                await dbContext.Tags
                    .Where(t => tagIds.Contains(t.Id))
                    .CountAsync(cancellation) == tagIds.Count)
            .WithMessage("One or more Tag IDs are invalid");
    }
}

