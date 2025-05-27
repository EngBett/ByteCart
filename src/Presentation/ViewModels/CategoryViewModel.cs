using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels;

public class CategoryViewModel
{
    public string? Id { get; set; }
    [Required]public string Name { get; set; } = string.Empty;
    [Required]public string? Description { get; set; }
    public string? ParentCategoryId { get; set; }
}
