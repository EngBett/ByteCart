using ByteCart.Application.Categories;
using ByteCart.Application.Categories.Queries.GetCategories;
using ByteCart.Application.Products.Commands.CreateProduct;
using ByteCart.Application.Products.Commands.UpdateProduct;
using ByteCart.Application.Products.Queries.GetProducts;
using ByteCart.Application.Suppliers;
using ByteCart.Application.Suppliers.Queries.GetSuppliers;
using ByteCart.Application.Tags;
using ByteCart.Application.Tags.Queries.GetTags;
using ByteCart.Domain.Enums;
using Microsoft.AspNetCore.Components;

namespace Presentation.Components.Pages.Products;

public partial class EditProduct
{
    [Parameter]
    public string Id { get; set; } = string.Empty;

    private List<CategoryDto> categories = new();
    private List<TagDto> tags = new();
    private List<SupplierDto> suppliers = new();
    private bool loading = false;
    private string alertMessage = string.Empty;
    private string alertType = "info";
    private string? productThumbnailUrl;

    // Form model
    private CreateProductCommand productModel = new();
    private Dictionary<string, bool> categorySelections = new();
    private Dictionary<string, bool> tagSelections = new();

    protected override async Task OnInitializedAsync()
    {
        loading = true;
        
        try
        {
            await LoadFilterOptionsAsync();
            await LoadProductAsync();
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

    private async Task LoadProductAsync()
    {
        try
        {
            var productQuery = new GetProductsQuery { ProductId = Id };
            var result = await Mediator.Send(productQuery);
            var product = result.Items.FirstOrDefault();
            
            if (product == null)
            {
                alertMessage = "Product not found";
                alertType = "warning";
                NavigationManager.NavigateTo("/products");
                return;
            }
            
            // Set the thumbnail URL
            productThumbnailUrl = product.ThumbnailUrl;
            
            productModel = new CreateProductCommand
            {
                Name = product.Name,
                SKU = product.SKU,
                Description = product.Description,
                Price = product.Price,
                CostPrice = product.CostPrice,
                StockQuantity = product.StockQuantity,
                Status = Enum.Parse<ProductStatus>(product.Status.ToString()),
                SupplierId = product.SupplierId ?? string.Empty,
                LaunchDate = product.LaunchDate,
                EndDate = product.EndDate
            };
            
            // Set the selections based on product data
            foreach (var categoryName in product.CategoryNames)
            {
                var category = categories.FirstOrDefault(c => c.Name == categoryName);
                if (category != null && categorySelections.ContainsKey(category.Id))
                {
                    categorySelections[category.Id] = true;
                }
            }
            
            foreach (var tagName in product.TagNames)
            {
                var tag = tags.FirstOrDefault(t => t.Name == tagName);
                if (tag != null && tagSelections.ContainsKey(tag.Id))
                {
                    tagSelections[tag.Id] = true;
                }
            }
        }
        catch (Exception ex)
        {
            alertMessage = $"Error loading product details: {ex.Message}";
            alertType = "danger";
        }
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
            
            var updateCommand = new UpdateProductCommand
            {
                Id = Id,
                Name = productModel.Name,
                SKU = productModel.SKU,
                Description = productModel.Description,
                Price = productModel.Price,
                CostPrice = productModel.CostPrice,
                StockQuantity = productModel.StockQuantity,
                Status = productModel.Status,
                SupplierId = productModel.SupplierId,
                LaunchDate = productModel.LaunchDate,
                EndDate = productModel.EndDate,
                CategoryIds = productModel.CategoryIds ?? new List<string>(),
                TagIds = productModel.TagIds
            };
            
            var result = await Mediator.Send(updateCommand);
            alertMessage = "Product updated successfully";
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
    
    private void OnProductImageUploaded(string imageUrl)
    {
        productThumbnailUrl = imageUrl;
        StateHasChanged();
    }
}
