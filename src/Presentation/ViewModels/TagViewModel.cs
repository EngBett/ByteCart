using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels;

public class TagViewModel
{
    public string? Id { get; set; }
    [Required]public string Name { get; set; } = null!;
}
