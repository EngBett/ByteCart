using ByteCart.Application.Common.Interfaces;
using ByteCart.Application.Products.Models;

namespace ByteCart.Application.Dashboard.Queries.GetPlatformSummaries;

public record GetPlatformSummariesQuery : IRequest<DashboardSummaryVm>;

public class GetPlatformSummariesQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetPlatformSummariesQuery, DashboardSummaryVm>
{
    public async Task<DashboardSummaryVm> Handle(GetPlatformSummariesQuery request, CancellationToken cancellationToken)
    {
        var productsCount = await context.Products.CountAsync(cancellationToken);
        var categoriesCount = await context.Categories.CountAsync(cancellationToken);
        var suppliersCount = await context.Suppliers.CountAsync(cancellationToken);
        var totalStock = await context.Products.SumAsync(p => p.StockQuantity, cancellationToken);

        var recentlyAddedProducts = await context.Products
            .AsNoTracking()
            .Include(x=>x.Images)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5).ProjectTo<ProductDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
        
        return new DashboardSummaryVm
        {
            Products = productsCount,
            Categories = categoriesCount,
            Suppliers = suppliersCount,
            TotalStock = totalStock,
            RecentlyAddedProducts = recentlyAddedProducts
        };
    }
}
