using ByteCart.Domain.Entities;

namespace ByteCart.Application.Common.Interfaces;

public interface IApplicationDbContext
{

    public DbSet<Product> Products { get; }
    public DbSet<ProductImage> ProductImages { get; }
    public DbSet<Supplier> Suppliers { get; }
    public DbSet<Tag> Tags { get; }
    public DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
