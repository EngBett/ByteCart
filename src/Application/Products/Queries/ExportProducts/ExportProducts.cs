using ByteCart.Application.Common.Interfaces;
using ByteCart.Application.Products.Models;
using ByteCart.Domain.Enums;

namespace ByteCart.Application.Products.Queries.ExportProducts;

public record ExportProductsQuery : IRequest<ExportProductsVm>
{
    public string? ProductId { get; set; }
    public string? SearchTerm { get; set; }
    public List<string>? CategoryIds { get; set; }
    public List<string>? TagIds { get; set; }
    public string? SupplierId { get; set; }
    public ProductStatus? Status { get; set; }

    public ProductSortBy? SortBy { get; set; } = ProductSortBy.DateCreated;
    public bool SortDescending { get; set; } = true;
}

public class ExportProductsQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
    : IRequestHandler<ExportProductsQuery, ExportProductsVm>
{
    public async Task<ExportProductsVm> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        
        var query = context.Products
            .AsNoTracking()
            .Where(x => string.IsNullOrWhiteSpace(request.ProductId) || x.Id == request.ProductId)
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
            _ => query.OrderByDescending(p => p.CreatedAt).ThenBy(p => p.Id)
        };

        var products = await query
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var fileContent = fileBuilder.BuildProductsFile(products);

        var fileName = $"Products-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv";

        return new ExportProductsVm
        {
            Content = fileContent,
            ContentType = "text/csv",
            FileName = fileName
        };
    }
}
