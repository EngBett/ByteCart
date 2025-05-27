using ByteCart.Application.Common.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Presentation.Components.Pages;

public class DashboardComponentBase : ComponentBase
{
    [Inject] public IMediator Mediator { get; set; } = null!;

    [Inject] public ICurrentUser CurrentUserService { get; set; } = null!;
    [Inject] public IJSRuntime Js { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await Js.InvokeVoidAsync("localStorage.setItem", "isLoggedIn", "{\"isLoggedIn\":true}");
    }
}
