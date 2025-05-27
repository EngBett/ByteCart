namespace ByteCart.Domain.Entities;

public class Supplier : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    public string? Website { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
