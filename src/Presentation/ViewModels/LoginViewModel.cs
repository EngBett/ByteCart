using System.ComponentModel.DataAnnotations;

namespace Presentation.ViewModels;

public class LoginViewModel
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    public bool RememberMe { get; set; }
}
