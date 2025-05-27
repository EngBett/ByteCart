using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace Presentation.Services;

public class CustomAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(
        ILoggerFactory loggerFactory, 
        IHttpContextAccessor httpContextAccessor) 
        : base(loggerFactory)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // You could add additional validation logic here if needed
        // For example, check if the user still exists in the database or if their roles have changed
        return Task.FromResult(true);
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity ?? new ClaimsIdentity())));
    }
}
