using ByteCart.Application.Suppliers;
using Microsoft.AspNetCore.Components;

namespace Presentation.Components.Pages.Suppliers.Components;

public partial class SupplierTableComponent
{
    [Parameter] public IEnumerable<SupplierDto> Suppliers { get; set; } = [];
    [Parameter] public EventCallback<SupplierDto> OnEdit { get; set; }
    [Parameter] public EventCallback<SupplierDto> OnDelete { get; set; }

    private async Task EditSupplier(SupplierDto supplier)
    {
        await OnEdit.InvokeAsync(supplier);
    }

    private async Task DeleteSupplier(SupplierDto supplier)
    {
        await OnDelete.InvokeAsync(supplier);
    }
}

