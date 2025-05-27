using ByteCart.Application.Common.Interfaces;
using ByteCart.Application.Common.Mappings;
using ByteCart.Application.Common.Models;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery : IRequest<PaginatedList<SupplierDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SupplierId { get; set; }
    public SupplierSortBy? SortBy { get; set; } = SupplierSortBy.DateCreated;
    public bool SortDescending { get; set; } = true;
}

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, PaginatedList<SupplierDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSuppliersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Supplier> query = _context.Suppliers
            .AsNoTracking()
            .Where(
                x =>
                    (string.IsNullOrWhiteSpace(request.SearchTerm) || x.Name.Contains(request.SearchTerm)) &&
                    (string.IsNullOrWhiteSpace(request.SupplierId) || x.Id == request.SupplierId)
            );
        
        query = request.SortBy switch
        {
            SupplierSortBy.Name => request.SortDescending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),
            SupplierSortBy.DateCreated => request.SortDescending
                ? query.OrderByDescending(x => x.CreatedAt)
                : query.OrderBy(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        return await query
            .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
