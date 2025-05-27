using ByteCart.Application.Common.Interfaces;
using ByteCart.Application.Common.Mappings;
using ByteCart.Application.Common.Models;
using ByteCart.Application.Products.Models;
using ByteCart.Domain.Enums;

namespace ByteCart.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<PaginatedList<ProductDto>>
{
    public string? ProductId { get; set; }
    public string? SearchTerm { get; set; }
    public List<string>? CategoryIds { get; set; }
    public List<string>? TagIds { get; set; }
    public string? SupplierId { get; set; }
    public ProductStatus? Status { get; set; }

    public ProductSortBy? SortBy { get; set; } = ProductSortBy.DateCreated;
    public bool SortDescending { get; set; } = true;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetProductsQueryQueryHandler : IRequestHandler<GetProductsQuery,
    PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsQueryQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(x => string.IsNullOrWhiteSpace(request.ProductId) || x.Id == request.ProductId)
            .Include(p => p.Images)
            .Include(p => p.Categories)
            .Include(p => p.Tags)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.SKU.ToLower().Contains(search));
        }

        if (request.CategoryIds?.Any() == true)
        {
            query = query.Where(p =>
                p.Categories.Any(c => request.CategoryIds.Contains(c.Id)));
        }

        if (request.TagIds?.Any() == true)
        {
            query = query.Where(p =>
                p.Tags.Any(t => request.TagIds.Contains(t.Id)));
        }

        if (!string.IsNullOrWhiteSpace(request.SupplierId))
        {
            query = query.Where(p => p.SupplierId == request.SupplierId);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }
        
        query = request.SortBy switch
        {
            ProductSortBy.Price => request.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            ProductSortBy.Stock => request.SortDescending
                ? query.OrderByDescending(p => p.StockQuantity)
                : query.OrderBy(p => p.StockQuantity),
            ProductSortBy.DateCreated => request.SortDescending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id) // Add a secondary sort key for deterministic ordering
        };

        return await query.ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
