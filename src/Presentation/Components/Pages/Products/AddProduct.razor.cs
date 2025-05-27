using System.ComponentModel.DataAnnotations;
using ByteCart.Application.Categories;
using ByteCart.Application.Categories.Queries.GetCategories;
using ByteCart.Application.Products.Commands.CreateProduct;
using ByteCart.Application.Suppliers;
using ByteCart.Application.Suppliers.Queries.GetSuppliers;
using ByteCart.Application.Tags;
using ByteCart.Application.Tags.Queries.GetTags;
using ByteCart.Domain.Enums;
using Microsoft.AspNetCore.Components;

namespace Presentation.Components.Pages.Products;

public partial class AddProduct
{
    private List<CategoryDto> categories = new();
    private List<TagDto> tags = new();
    private List<SupplierDto> suppliers = new();
    private bool loading = false;
    private string alertMessage = string.Empty;
    private string alertType = "info";

    // Form model
    private Model productModel = new();
    private Dictionary<string, bool> categorySelections = new();
    private Dictionary<string, bool> tagSelections = new();

    protected override async Task OnInitializedAsync()
    {
        loading = true;

        try
        {
            await LoadFilterOptionsAsync();
        }
        finally
        {
            loading = false;
        }
    }

    private async Task LoadFilterOptionsAsync()
    {
        // Load categories
        var categoriesResult = await Mediator.Send(new GetCategoriesQuery { PageSize = 100 });
        categories = categoriesResult.Items.ToList();

        // Load tags
        var tagsResult = await Mediator.Send(new GetTagsQuery());
        tags = tagsResult.ToList();

        // Load suppliers
        var suppliersResult = await Mediator.Send(new GetSuppliersQuery { PageSize = 100 });
        suppliers = suppliersResult.Items.ToList();

        // Initialize selections
        InitializeCategoryAndTagSelections();
    }

    private void InitializeCategoryAndTagSelections()
    {
        // Initialize all to false
        categorySelections = categories.ToDictionary(c => c.Id, _ => false);
        tagSelections = tags.ToDictionary(t => t.Id, _ => false);
    }

    private async Task SaveProduct()
    {
        try
        {
            loading = true;

            // Set selected category and tag IDs
            productModel.CategoryIds = categorySelections
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();

            productModel.TagIds = tagSelections
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();

            _ = await Mediator.Send(productModel.ToCreateProductCommand());
            alertMessage = "Product created successfully";
            alertType = "success";

            // Navigate back to products list
            NavigationManager.NavigateTo("/products");
        }
        catch (Exception ex)
        {
            alertMessage = $"Error saving product: {ex.Message}";
            alertType = "danger";
        }
        finally
        {
            loading = false;
        }
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/products");
    }

    private void OnCategorySelectionChanged(string categoryId, ChangeEventArgs args)
    {
        if (args.Value is bool isChecked)
        {
            categorySelections[categoryId] = isChecked;
        }
    }

    private void OnTagSelectionChanged(string tagId, ChangeEventArgs args)
    {
        if (args.Value is bool isChecked)
        {
            tagSelections[tagId] = isChecked;
        }
    }

    private class Model
    {
        [Required] public string Name { get; set; } = null!;
        [Required] public string SKU { get; set; } = null!;
        [Required] public string Description { get; set; } = null!;
        [Required] public decimal Price { get; set; }
        [Required] public decimal CostPrice { get; set; }
        [Required] public int StockQuantity { get; set; }
        [Required] public ProductStatus Status { get; set; }
        [Required] public string SupplierId { get; set; } = null!;
        public DateTime? LaunchDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string>? TagIds { get; set; }
        public List<string>? CategoryIds { get; set; }

        public CreateProductCommand ToCreateProductCommand() => new CreateProductCommand
        {
            Name = Name,
            SKU = SKU,
            Description = Description,
            Price = Price,
            CostPrice = CostPrice,
            StockQuantity = StockQuantity,
            Status = Status,
            SupplierId = SupplierId,
            LaunchDate = LaunchDate.HasValue ? DateTime.SpecifyKind(LaunchDate.Value, DateTimeKind.Utc) : null,
            EndDate = EndDate.HasValue ? DateTime.SpecifyKind(EndDate.Value, DateTimeKind.Utc) : null,
            TagIds = TagIds,
            CategoryIds = CategoryIds,
        };
    }
}
