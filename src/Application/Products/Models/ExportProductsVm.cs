namespace ByteCart.Application.Products.Models;

public class ExportProductsVm
{
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}
