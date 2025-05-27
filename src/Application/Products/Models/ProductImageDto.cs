using ByteCart.Domain.Entities;

namespace ByteCart.Application.Products.Models;

public class ProductImageDto
{
    public string Id { get; set; } = null!;
    public string Url { get; set; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ProductImage, ProductImageDto>();
        }
    }
}
