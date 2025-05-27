using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand : IRequest<string>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ParentCategoryId { get; set; }
}


public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
