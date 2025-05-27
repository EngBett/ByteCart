using ByteCart.Domain.Constants;
using ByteCart.Domain.Entities;
using ByteCart.Domain.Enums;
using ByteCart.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ByteCart.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "admin@demo.dev", Email = "admin@demo.dev" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Password123!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, [administratorRole.Name]);
            }
        }

        // Check if we need to seed sample data
        if (!_context.Suppliers.Any() && !_context.Categories.Any() && !_context.Tags.Any())
        {
            await SeedSampleDataAsync();
        }
    }
    
    private async Task SeedSampleDataAsync()
    {
        _logger.LogInformation("Seeding sample data...");
        
        // 1. Create 5 suppliers
        var suppliers = new List<Supplier>();
        for (int i = 1; i <= 5; i++)
        {
            var supplier = new Supplier
            {
                Name = $"Supplier {i}",
                ContactEmail = $"contact@supplier{i}.com",
                ContactNumber = $"+1-555-000-{1000 + i}",
                Website = $"https://supplier{i}.com"
            };
            suppliers.Add(supplier);
        }
        await _context.Suppliers.AddRangeAsync(suppliers);
        
        // 2. Create 5 tags
        var tags = new List<Tag>();
        for (int i = 1; i <= 5; i++)
        {
            var tag = new Tag
            {
                Name = $"Tag {i}"
            };
            tags.Add(tag);
        }
        await _context.Tags.AddRangeAsync(tags);
        
        // 3. Create 5 categories
        var categories = new List<Category>();
        for (int i = 1; i <= 5; i++)
        {
            var category = new Category
            {
                Name = $"Category {i}",
                Description = $"Description for Category {i}"
            };
            categories.Add(category);
        }
        await _context.Categories.AddRangeAsync(categories);
        
        // Save to generate IDs
        await _context.SaveChangesAsync();
        
        // 4. Create 5 products per category (total 25 products)
        var random = new Random();
        var products = new List<Product>();
        
        foreach (var category in categories)
        {
            for (int i = 1; i <= 5; i++)
            {
                // Randomly select supplier
                var supplier = suppliers[random.Next(suppliers.Count)];
                
                // Create product
                var product = new Product
                {
                    Name = $"{category.Name} - Product {i}",
                    SKU = $"SKU-{category.Name.Replace(" ", "")}-{i}",
                    Description = $"This is product {i} in the {category.Name} category",
                    Price = Math.Round((decimal)(random.NextDouble() * 100 + 10), 2), // Random price between 10 and 110
                    CostPrice = Math.Round((decimal)(random.NextDouble() * 50 + 5), 2), // Random cost between 5 and 55
                    StockQuantity = random.Next(10, 100),
                    Status = (ProductStatus)random.Next(3), // Random status
                    SupplierId = supplier.Id,
                    LaunchDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                };
                
                // Add to category
                product.Categories.Add(category);
                
                // Add 1-3 random tags
                int numTags = random.Next(1, 4);
                var shuffledTags = tags.OrderBy(_ => random.Next()).Take(numTags).ToList();
                
                foreach (var tag in shuffledTags)
                {
                    product.Tags.Add(tag);
                }
                
                products.Add(product);
            }
        }
        
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Sample data seeding completed.");
    }
}
