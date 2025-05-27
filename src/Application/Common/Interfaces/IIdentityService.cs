using System.Security.Claims;
using ByteCart.Application.Common.Models;

namespace ByteCart.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);
    Task<List<Claim>> GetUserClaimsAsync(string username);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);
    Task<bool> CheckPasswordAsync(string username, string password);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);
}
