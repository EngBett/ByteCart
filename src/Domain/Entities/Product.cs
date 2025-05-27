namespace ByteCart.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string SupplierId { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}
