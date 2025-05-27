using ByteCart.Application.Suppliers;
using Microsoft.AspNetCore.Components;
using Presentation.ViewModels;

namespace Presentation.Components.Pages.Suppliers.Components;

public partial class SupplierModalComponent
{
    [Parameter] public bool ShowModal { get; set; }
    [Parameter] public string ModalTitle { get; set; } = "Create Supplier";
    [Parameter] public SupplierViewModel SupplierModel { get; set; } = new();
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public EventCallback OnSave { get; set; }

    private async Task SaveSupplier()
    {
        await OnSave.InvokeAsync(SupplierModel);
        await CloseModal();
    }

    private async Task CloseModal()
    {
        ShowModal = false;
        await OnCancel.InvokeAsync(ShowModal);
        SupplierModel = new SupplierViewModel();
    }
}

