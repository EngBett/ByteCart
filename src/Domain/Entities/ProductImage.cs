namespace ByteCart.Domain.Entities;

public class ProductImage : BaseAuditableEntity
{
    public string Url { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
