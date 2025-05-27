using Microsoft.AspNetCore.Components;

namespace Presentation.Components.Pages.Home.Components;

public partial class QuickActionsItemComponent
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string Href { get; set; } = string.Empty;
    
    [Parameter]
    public string IconifyIcon { get; set; } = string.Empty;

    [Parameter]
    public string? Description { get; set; }

    [Parameter]
    public EventCallback OnClick { get; set; }

    private async Task HandleClick()
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }
    }
}

