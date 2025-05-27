using Microsoft.AspNetCore.Components;

namespace Presentation.Components.ReusableComponents;

public partial class DeleteConfirmationComponent
{
    [Parameter] public bool ShowModal { get; set; }
    [Parameter] public string ModalTitle { get; set; } = "";
    [Parameter] public string? Message { get; set; }
    [Parameter] public EventCallback<bool> OnConfirm { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private async Task ConfirmDelete()
    {
        await OnConfirm.InvokeAsync(true);
        await CloseModal();
    }

    private async Task CloseModal()
    {
        ShowModal = false;
        await OnCancel.InvokeAsync();
    }
}

