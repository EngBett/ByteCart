using ByteCart.Application.Categories;
using ByteCart.Application.Categories.Queries.GetCategories;
using ByteCart.Application.Common.Models;
using ByteCart.Application.Products;
using ByteCart.Application.Products.Commands.DeleteProduct;
using ByteCart.Application.Products.Models;
using ByteCart.Application.Products.Queries.ExportProducts;
using ByteCart.Application.Products.Queries.GetProducts;
using ByteCart.Application.Suppliers;
using ByteCart.Application.Suppliers.Queries.GetSuppliers;
using ByteCart.Application.Tags;
using ByteCart.Application.Tags.Queries.GetTags;
using ByteCart.Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace Presentation.Components.Pages.Products;

public partial class Products
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    private PaginatedList<ProductDto>? products;
    private List<CategoryDto> categories = new();
    private List<TagDto> tags = new();
    private List<SupplierDto> suppliers = new();
    private bool loading = true;
    private string alertMessage = string.Empty;
    private string alertType = "info";

    // Filter and Sort parameters
    private string searchTerm = string.Empty;
    private string selectedCategoryId = string.Empty;
    private string selectedTagId = string.Empty;
    private string selectedSupplierId = string.Empty;
    private string selectedStatusString = string.Empty;
    private ProductSortBy sortBy = ProductSortBy.DateCreated;
    private bool sortDesc = true;
    private int currentPage = 1;
    private const int PageSize = 10;


    // Delete confirmation
    private bool showDeleteConfirmation = false;
    private string? productToDeleteId;
    private string? productToDeleteName;

    protected override async Task OnInitializedAsync()
    {
        await LoadFilterOptionsAsync();
        await LoadProductsAsync();
    }

    private async Task LoadProductsAsync()
    {
        loading = true;
        try
        {
            ProductStatus? status = null;
            if (!string.IsNullOrEmpty(selectedStatusString))
            {
                if (Enum.TryParse<ProductStatus>(selectedStatusString, out var parsedStatus))
                {
                    status = parsedStatus;
                }
            }

            var query = new GetProductsQuery
            {
                PageNumber = currentPage,
                PageSize = PageSize,
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm,
                CategoryIds =
                    string.IsNullOrEmpty(selectedCategoryId) ? null : new List<string> { selectedCategoryId },
                TagIds = string.IsNullOrEmpty(selectedTagId) ? null : new List<string> { selectedTagId },
                SupplierId = string.IsNullOrEmpty(selectedSupplierId) ? null : selectedSupplierId,
                Status = status,
                SortBy = sortBy,
                SortDescending = sortDesc
            };

            products = await Mediator.Send(query);
        }
        catch (Exception ex)
        {
            alertMessage = $"Error loading products: {ex.Message}";
            alertType = "danger";
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
    }

    private async Task HandlePageChangeAsync(int page)
    {
        currentPage = page;
        await LoadProductsAsync();
    }

    private async Task ApplyFilters()
    {
        currentPage = 1;
        await LoadProductsAsync();
    }

    private async Task ResetFilters()
    {
        searchTerm = string.Empty;
        selectedCategoryId = string.Empty;
        selectedTagId = string.Empty;
        selectedSupplierId = string.Empty;
        selectedStatusString = string.Empty;
        sortBy = ProductSortBy.DateCreated;
        sortDesc = true;
        currentPage = 1;

        await LoadProductsAsync();
    }

    private void NavigateToAddProduct()
    {
        NavigationManager.NavigateTo("/products/add");
    }

    private void NavigateToEditProduct(string productId)
    {
        NavigationManager.NavigateTo($"/products/edit/{productId}");
    }

    private void ShowDeleteConfirmation(string id, string name)
    {
        productToDeleteId = id;
        productToDeleteName = name;
        showDeleteConfirmation = true;
    }

    private void CancelDelete()
    {
        showDeleteConfirmation = false;
    }

    private async Task DeleteProduct()
    {
        try
        {
            await Mediator.Send(new DeleteProductCommand { Id = productToDeleteId ?? string.Empty });
            alertMessage = "Product deleted successfully";
            alertType = "success";
            showDeleteConfirmation = false;
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error deleting product: {ex.Message}";
            alertType = "danger";
        }
    }

    private async Task ExportProductsToCSV()
    {
        try
        {
            loading = true;
            
            // Get current filter parameters to apply to export
            ProductStatus? status = null;
            if (!string.IsNullOrEmpty(selectedStatusString))
            {
                if (Enum.TryParse<ProductStatus>(selectedStatusString, out var parsedStatus))
                {
                    status = parsedStatus;
                }
            }

            var query = new ExportProductsQuery
            {
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm,
                CategoryIds = string.IsNullOrEmpty(selectedCategoryId) ? null : new List<string> { selectedCategoryId },
                TagIds = string.IsNullOrEmpty(selectedTagId) ? null : new List<string> { selectedTagId },
                SupplierId = string.IsNullOrEmpty(selectedSupplierId) ? null : selectedSupplierId,
                Status = status,
                SortBy = sortBy,
                SortDescending = sortDesc
            };

            // Execute the query to get the CSV file
            var result = await Mediator.Send(query);
            
            // Convert the byte array to a base64 string
            var base64 = Convert.ToBase64String(result.Content);
            
            // Create a data URL
            var dataUrl = $"data:{result.ContentType};base64,{base64}";
            
            // Use browser's download API via JS interop
            await JSRuntime.InvokeVoidAsync("window.downloadFileFromUrl", dataUrl);
            
            alertMessage = "Products exported successfully";
            alertType = "success";
        }
        catch (Exception ex)
        {
            alertMessage = $"Error exporting products: {ex.Message}";
            alertType = "danger";
        }
        finally
        {
            loading = false;
            StateHasChanged();
        }
    }
}
