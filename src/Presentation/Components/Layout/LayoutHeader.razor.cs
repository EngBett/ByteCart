using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Presentation.Components.Layout;

public partial class LayoutHeader
{
    [Parameter]public bool MenuVisible { get; set; }
    
    [Parameter] public EventCallback<bool> MenuStateChanged { get; set; }
    [Parameter] public IJSRuntime Js { get; set; } = null!;
    [Inject] public IHttpContextAccessor ContextAccessor { get; set; } = null!;
    
    private bool ProfileDropdownVisible { get; set; }
    
    private bool isDarkMode = false;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Check current dark mode status using our JS theme manager
            try {
                isDarkMode = await Js.InvokeAsync<bool>("themeManager.isDarkMode");
                
                // Force UI update since we modified state
                StateHasChanged();
            }
            catch {
                // Fallback if JS interop fails
                var theme = await Js.InvokeAsync<string>("localStorage.getItem", "theme");
                isDarkMode = theme == "dark";
                StateHasChanged();
            }
        }
    }

    private async Task ToggleTheme()
    {
        try {
           
            await Js.InvokeVoidAsync("themeManager.toggle");
            
            // Update our local state
            isDarkMode = await Js.InvokeAsync<bool>("themeManager.isDarkMode");
        }
        catch {
            // Fallback approach if JS interop fails
            isDarkMode = !isDarkMode;

            if (isDarkMode)
            {
                await Js.InvokeVoidAsync("document.documentElement.classList.add", "dark");
                await Js.InvokeVoidAsync("localStorage.setItem", "theme", "dark");
            }
            else
            {
                await Js.InvokeVoidAsync("document.documentElement.classList.remove", "dark");
                await Js.InvokeVoidAsync("localStorage.setItem", "theme", "light");
            }
            
            await Js.InvokeVoidAsync("initializeFlowbite");
        }
    }
    
    private async Task OnOpenMenuAsync(MouseEventArgs e)
    {
        MenuVisible = true;
        await MenuStateChanged.InvokeAsync(true);
    }
}
