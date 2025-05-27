using Microsoft.JSInterop;

namespace Presentation.Components.Layout;

public partial class AccountLayout
{
    
    private bool ProfileDropdownVisible { get; set; }
    private bool MenuVisible { get; set; }
    private bool StkFormVisible { get; set; }
    
    protected override async Task OnAfterRenderAsync(bool isFirstRender) {
        if (isFirstRender)
        {
            await Js.InvokeVoidAsync("window.initializeFlowbite");
        }
    }
    
    protected override async Task OnInitializedAsync()
    {
        ProfileDropdownVisible = false;
        MenuVisible = false;
        StkFormVisible = false;
        await InvokeAsync(StateHasChanged);
    }

    private void StkFormStateChangedHandler(bool state)
    {
        StkFormVisible = state;
    }
    
    private void MenuStateChangedHandler(bool state)
    {
        MenuVisible = state;
    }
    
}
