using ByteCart.Domain.Entities;

namespace ByteCart.Application.Tags;

public class TagDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    
    private class Mapping:Profile
    {
        public Mapping()
        {
            CreateMap<Tag, TagDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name));
        }
    }
}
