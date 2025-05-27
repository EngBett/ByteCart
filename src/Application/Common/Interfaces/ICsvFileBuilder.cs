using ByteCart.Application.Products.Models;

namespace ByteCart.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildProductsFile(IEnumerable<ProductDto> products);
}
