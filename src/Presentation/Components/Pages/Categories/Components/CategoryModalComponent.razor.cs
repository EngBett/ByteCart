using ByteCart.Application.Categories;
using Microsoft.AspNetCore.Components;
using Presentation.ViewModels;

namespace Presentation.Components.Pages.Categories.Components;

public partial class CategoryModalComponent
{
    [Parameter] public bool ShowModal { get; set; }
    [Parameter] public bool IsEditMode { get; set; }
    [Parameter] public string ModalTitle { get; set; } = "Create Category";
    [Parameter] public List<CategoryDto>? AllCategories { get; set; } = [];
    [Parameter] public CategoryViewModel CategoryModel { get; set; } = new();
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }

    private bool _submitting { get; set; }

    private async Task SaveCategory()
    {
        if (_submitting) return;
        _submitting = true;
        await OnSave.InvokeAsync(CategoryModel);
        await CloseModal();
        _submitting = false;
    }

    private async Task CloseModal()
    {
        ShowModal = false;
        await OnCancel.InvokeAsync(ShowModal);
        CategoryModel = new CategoryViewModel();
    }
}
