using ByteCart.Domain.Entities;

namespace ByteCart.Application.Categories;

public class CategoryDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName,
                    opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));
        }
    }
}
