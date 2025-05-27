using Microsoft.AspNetCore.Components;

namespace Presentation.Components.ReusableComponents;

public partial class HeaderComponent
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string Subtitle { get; set; } = string.Empty;
    
    [Parameter]
    public string ButtonName { get; set; } = string.Empty;

    [Parameter]
    public string? IconifyIcon { get; set; } = string.Empty;

    [Parameter]
    public EventCallback OnActionClick { get; set; }

    private async Task HandleActionClick()
    {
        if (OnActionClick.HasDelegate)
        {
            await OnActionClick.InvokeAsync();
        }
    }
}

