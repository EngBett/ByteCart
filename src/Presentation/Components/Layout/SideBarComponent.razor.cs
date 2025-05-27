using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Presentation.Components.Layout;

public partial class SideBarComponent
{
    [Parameter]public bool MenuVisible { get; set; }
    
    [Parameter] public EventCallback<bool> MenuStateChanged { get; set; }
    
    private async Task OnCloseMenuAsync(MouseEventArgs e)
    {
        await MenuStateChanged.InvokeAsync(false);
    }
}
