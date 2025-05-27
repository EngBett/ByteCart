using System.ComponentModel.DataAnnotations;

namespace Presentation.Components.Pages.Auth;

public partial class Register
{
    private Model _model = new();
    private bool _isLoading { get; set; }
    private string? alertMessage { get; set; }
    private string? alertType { get; set; }

    private async Task RegisterAsync()
    {
        if (_isLoading) return;
        _isLoading = true;
        try
        {
            var response = await IdentityService.CreateUserAsync(_model.Email, _model.Password);
            if (!response.Result.Succeeded)
            {
                SetErrorAlert(response.Result.Errors.FirstOrDefault() ?? "Registration failed.");
            }
            else
            {
                NavManager.NavigateTo("/auth/login",true);
            }
        }
        catch (Exception)
        {
            SetErrorAlert("An error occurred while registering. Please try again.");
        }
        _isLoading = false;
        StateHasChanged();
    }
    
    private void SetErrorAlert(string message, string type = "danger")
    {
        alertMessage = message;
        alertType = type;
        StateHasChanged();
    }

    private class Model
    {
        [Required] [EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
        [Required] [Compare("Password")] public string ConfirmPassword { get; set; } = null!;
    }
}
