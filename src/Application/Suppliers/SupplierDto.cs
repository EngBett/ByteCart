using ByteCart.Application.Products;
using ByteCart.Application.Products.Models;
using ByteCart.Domain.Entities;

namespace ByteCart.Application.Suppliers;

public class SupplierDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ContactNumber { get; set; }
    public string? Website { get; set; }
    public ICollection<ProductDto>? Products { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Supplier, SupplierDto>()
                .ForMember(dest => dest.Products, opt => opt
                    .MapFrom(src => src.Products));
            CreateMap<Product, ProductDto>();
        }
    }
}
