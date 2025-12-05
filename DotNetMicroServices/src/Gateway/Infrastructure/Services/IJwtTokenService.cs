using System.Security.Claims;

namespace Gateway.Infrastructure.Services;

public interface IJwtTokenService
{
    string? GetUserIdFromClaims(ClaimsPrincipal user);
    string? GetUserRoleFromClaims(ClaimsPrincipal user);
    string? GetUserIdFromToken(string token);
    string? GetUserRoleFromToken(string token);
}

