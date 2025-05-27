using ByteCart.Application.Common.Models;
using Microsoft.AspNetCore.Components;

namespace Presentation.Components.ReusableComponents;

public partial class PaginationComponent<T>
{
    [Parameter] public PaginatedList<T> Data { get; set; } = null!;

    [Parameter]
    public int CurrentPage { get; set; } = 1;
    
    [Parameter]
    public int PageSize { get; set; } = 10;

    [Parameter]
    public EventCallback<int> OnPageChanged { get; set; }

    private async Task HandlePageChangeAsync(int page)
    {
        CurrentPage = page;
        await OnPageChanged.InvokeAsync(CurrentPage);
    }
}
