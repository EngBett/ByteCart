using ByteCart.Application.Common.Interfaces;
using ByteCart.Domain.Entities;
using ByteCart.Domain.Enums;

namespace ByteCart.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand : IRequest<string>
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string Description { get; set; }= null!;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public ProductStatus Status { get; set; }
    public string SupplierId { get; set; } = null!;
    public DateTime? LaunchDate { get; set; }
    public DateTime? EndDate { get; set; }

    public List<string> CategoryIds { get; set; } = new();
    public List<string>? TagIds { get; set; }
}

public class UpdateProductCommandCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProductCommand, string>
{
    public async Task<string> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null) throw new NotFoundException(nameof(Product), request.Id);

        product.Name = request.Name;
        product.SKU = request.SKU;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CostPrice = request.CostPrice;
        product.StockQuantity = request.StockQuantity;
        product.Status = request.Status;
        product.SupplierId = request.SupplierId;
        product.LaunchDate = request.LaunchDate;
        product.EndDate = request.EndDate;
        product.LastModified = DateTime.UtcNow;

        var categories = await context.Categories
            .Where(c => request.CategoryIds.Contains(c.Id)).ToListAsync(cancellationToken);

        var tags = request.TagIds?.Any() == true
            ? await context.Tags.Where(t => request.TagIds.Contains(t.Id)).ToListAsync(cancellationToken)
            : [];

        product.Categories = categories;
        product.Tags = tags;

        await context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
