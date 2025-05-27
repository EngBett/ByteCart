using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;
using ByteCart.Domain.Enums;

namespace ByteCart.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<string>
{
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public ProductStatus Status { get; set; }
    public string SupplierId { get; set; } = null!;
    public DateTime? LaunchDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? TagIds { get; set; }
    public List<string>? CategoryIds { get; set; }
}

public class CreateProductCommandCommandHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            Price = request.Price,
            CostPrice = request.CostPrice,
            StockQuantity = request.StockQuantity,
            Status = request.Status,
            SupplierId = request.SupplierId,
            LaunchDate =
                request.LaunchDate.HasValue
                    ? DateTime.SpecifyKind(request.LaunchDate.Value, DateTimeKind.Utc)
                    : request.LaunchDate,
            EndDate = request.EndDate.HasValue
                ? DateTime.SpecifyKind(request.EndDate.Value, DateTimeKind.Utc)
                : request.EndDate,
        };

        if (request.TagIds?.Any() == true)
        {
            var tags = await _context.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync();
            product.Tags = tags;
        }

        if (request.CategoryIds?.Any() == true)
        {
            var categories = await _context.Categories.Where(c => request.CategoryIds.Contains(c.Id)).ToListAsync();
            product.Categories = categories;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
