using ByteCart.Application.Dashboard;
using ByteCart.Application.Dashboard.Queries.GetPlatformSummaries;

namespace Presentation.Components.Pages.Home;

public partial class Home
{
    private bool loading = false;
    private string alertMessage = string.Empty;
    private string alertType = "info";
    
    private DashboardSummaryVm _dashboardSummary = new();
    
    protected override async Task OnInitializedAsync()
    {
        await FetchSummaries();
    }
    
    private async Task FetchSummaries()
    {
        if(loading) return;
        loading = true;
        
        try
        {
            _dashboardSummary = await Mediator.Send(new GetPlatformSummariesQuery());
            loading = false;
        }
        catch (Exception)
        {
            alertType = "danger";
            alertMessage = "Error fetching dashboard summary.";
        }
        
        loading = false;
        await InvokeAsync(StateHasChanged);
    }
}
