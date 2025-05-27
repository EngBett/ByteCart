using ByteCart.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace ByteCart.Application.Products.Models;

public class ProductDto
{
    public string Id { get; set; } = null!;
    public string SupplierId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string SKU { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public int StockQuantity { get; set; }
    public DateTime? LaunchDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? ThumbnailUrl { get; set; }

    public List<string> CategoryNames { get; set; } = new();
    public List<string> TagNames { get; set; } = new();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryNames,
                    opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)))
                .ForMember(dest => dest.TagNames,
                    opt => opt.MapFrom(src => src.Tags.Select(t => t.Name)))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => GetFirstImageUrl(src)));
        }

        private static string? GetFirstImageUrl(Product product)
        {
            if (product.Images == null || !product.Images.Any())
                return null;

            var image = product.Images.FirstOrDefault();
            return image?.Url;
        }
    }
}
