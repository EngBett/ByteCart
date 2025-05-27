using ByteCart.Application.Products.Models;

namespace ByteCart.Application.Dashboard;

public class DashboardSummaryVm
{
    public int Products { get; set; }
    public int Categories { get; set; }
    public int Suppliers { get; set; }
    public int TotalStock { get; set; }
    public IEnumerable<ProductDto> RecentlyAddedProducts { get; set; } = [];
}
