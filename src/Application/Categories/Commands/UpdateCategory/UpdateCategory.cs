using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand : IRequest<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ParentCategoryId { get; set; }
}

public class UpdateCategoryCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateCategoryCommand, string>
{
    public async Task<string> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.FindAsync([request.Id], cancellationToken);

        if (category == null)
            throw new NotFoundException(nameof(Category), request.Id);

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;

        await context.SaveChangesAsync(cancellationToken);
        return category.Id;
    }
}
