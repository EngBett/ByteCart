using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Presentation.Components.Layout;

public partial class NavMenuItem:IDisposable
{
    [Parameter] public string IconifyIcon { get; set; } = "solar:info-circle-broken";
    [Parameter] public string Name { get; set; } = "Example Name";
    [Parameter] public string Href { get; set; } = "/";
    [Parameter] public bool IsActive { get; set; } = false;
    
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        CheckActivePage();
        NavigationManager.LocationChanged += OnLocationChanged;
        await base.OnInitializedAsync();
    }
    
    private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        CheckActivePage();
        StateHasChanged();
    }
    
    private void CheckActivePage()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        IsActive = uri.AbsolutePath == Href;
    }
    
    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
