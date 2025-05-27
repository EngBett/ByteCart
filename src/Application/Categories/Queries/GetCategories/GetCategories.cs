using ByteCart.Application.Common.Interfaces;
using ByteCart.Application.Common.Mappings;
using ByteCart.Application.Common.Models;

namespace ByteCart.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<PaginatedList<CategoryDto>>
{
    public string? CategoryId { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCategoriesQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetCategoriesQuery, PaginatedList<CategoryDto>>
{
    public async Task<PaginatedList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .Where(x => (string.IsNullOrWhiteSpace(request.CategoryId) || x.Id == request.CategoryId)&&(string.IsNullOrWhiteSpace(request.SearchTerm) || request.SearchTerm!.ToLower().Contains(request.SearchTerm.ToLower())) )
            .Include(c => c.ParentCategory)
            .ProjectTo<CategoryDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
