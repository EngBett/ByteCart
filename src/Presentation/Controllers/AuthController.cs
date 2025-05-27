using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels;

namespace Presentation.Controllers;

public class AuthController : Controller
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthController(IIdentityService identityService,IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (_httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
        {
            return LocalRedirect("/");
        }

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromHeader] LoginViewModel request)
    {
        if (!ModelState.IsValid) return View(request);
        
        var passwordCorrect = await _identityService.CheckPasswordAsync(request.Email, request.Password);

        if (!passwordCorrect)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var claims = await _identityService.GetUserClaimsAsync(request.Email);
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return LocalRedirect("/");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}

