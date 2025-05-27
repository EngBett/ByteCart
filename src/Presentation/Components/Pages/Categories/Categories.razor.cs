using ByteCart.Application.Categories;
using ByteCart.Application.Categories.Commands.CreateCategory;
using ByteCart.Application.Categories.Commands.DeleteCategory;
using ByteCart.Application.Categories.Commands.UpdateCategory;
using ByteCart.Application.Categories.Queries.GetCategories;
using ByteCart.Application.Common.Models;
using Microsoft.AspNetCore.Components;
using Presentation.ViewModels;

namespace Presentation.Components.Pages.Categories;

public partial class Categories
{
    private PaginatedList<CategoryDto>? _categories;
    private List<CategoryDto>? _allCategories;
    private bool _loading = true;
    private bool _showCategoryModal = false;
    private bool _showDeleteModal = false;
    private bool _isEditMode = false;
    private CategoryDto? _categoryToDelete;
    private string alertMessage = string.Empty;
    private string alertType = "info";

    // Search
    private string searchTerm = string.Empty;

    private int currentPage = 1;
    private int pageSize = 10;

    private CategoryViewModel _categoryModel = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCategories();
        await LoadAllCategories();
    }

    private async Task LoadCategories(int pageNumber = 1)
    {
        _loading = true;
        currentPage = pageNumber;

        try
        {
            var query = new GetCategoriesQuery { PageNumber = pageNumber, PageSize = pageSize, SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm };
            _categories = await Mediator.Send(query);
        }
        catch (Exception ex)
        {
            alertMessage = $"Error loading categories: {ex.Message}";
            alertType = "danger";
        }
        finally
        {
            _loading = false;
        }
    }

    private async Task LoadAllCategories()
    {
        try
        {
            var query = new GetCategoriesQuery { PageSize = 100 };
            var result = await Mediator.Send(query);
            _allCategories = result.Items.ToList();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error loading categories: {ex.Message}";
            alertType = "danger";
        }
    }

    private void OpenCreateModal()
    {
        _isEditMode = false;
        _categoryModel = new CategoryViewModel();
        _showCategoryModal = true;
    }

    private void OpenEditModal(CategoryDto category)
    {
        _isEditMode = true;
        _categoryModel = new CategoryViewModel
        {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategoryId = category.ParentCategoryId
        };
        _showCategoryModal = true;
    }

    private void OpenDeleteModal(CategoryDto category)
    {
        _categoryToDelete = category;
        _showDeleteModal = true;
    }

    private void CloseModal() => _showCategoryModal = false;
    private void CloseDeleteModal() => _showDeleteModal = false;

    private async Task HandleCategorySubmit()
    {
        
        try
        {
            if (_isEditMode)
            {
                var command = new UpdateCategoryCommand
                {
                        Id = _categoryModel.Id!,
                        Name = _categoryModel.Name,
                        Description = _categoryModel.Description,
                        ParentCategoryId = _categoryModel.ParentCategoryId
                };
                await Mediator.Send(command);
                alertMessage = "Category updated successfully!";
            }
            else
            {
                var command = new CreateCategoryCommand
                {
                        Name = _categoryModel.Name,
                        Description = _categoryModel.Description,
                        ParentCategoryId = _categoryModel.ParentCategoryId
                };
                await Mediator.Send(command);
                alertMessage = "Category created successfully!";
            }

            alertType = "success";
            CloseModal();
            await LoadCategories();
            await LoadAllCategories();
        }
        catch (Exception ex)
        {
            alertMessage = $"Error saving category: {ex.Message}";
            alertType = "danger";
        }
        
    }

    private async Task DeleteCategory()
    {

        try
        {
            if (_categoryToDelete != null)
            {
                var command = new DeleteCategoryCommand { Id = _categoryToDelete.Id };
                await Mediator.Send(command);
                alertMessage = $"Category '{_categoryToDelete.Name}' deleted successfully!";
                alertType = "success";
                CloseDeleteModal();
                await LoadCategories();
                await LoadAllCategories();
            }
        }
        catch (Exception ex)
        {
            alertMessage = $"Error deleting category: {ex.Message}";
            alertType = "danger";
        }
    }
}
