namespace ByteCart.Domain.Entities;

public class Tag : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
