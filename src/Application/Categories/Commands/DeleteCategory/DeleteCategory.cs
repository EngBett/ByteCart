using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand : IRequest<bool>
{
    public string Id { get; set; } = null!;
}

public class DeleteCategoryCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);

        // Prevent deletion if products are assigned
        if (category.Products.Any())
            throw new ValidationException("Cannot delete a category that is assigned to one or more products.");

        // Recursively delete subcategories
        await DeleteSubcategoriesRecursive(category, cancellationToken);

        context.Categories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task DeleteSubcategoriesRecursive(Category category, CancellationToken cancellationToken)
    {
        foreach (var subCategory in category.SubCategories.ToList())
        {
            var fullSub = await context.Categories
                .Include(sc => sc.SubCategories)
                .Include(sc => sc.Products)
                .FirstOrDefaultAsync(sc => sc.Id == subCategory.Id, cancellationToken);

            if (fullSub == null) continue;

            if (fullSub.Products.Count != 0)
                throw new ValidationException($"Cannot delete subcategory '{fullSub.Name}' because it is assigned to one or more products.");

            await DeleteSubcategoriesRecursive(fullSub, cancellationToken);
            context.Categories.Remove(fullSub);
        }
    }
}
