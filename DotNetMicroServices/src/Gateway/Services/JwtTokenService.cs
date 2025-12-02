using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(ILogger<JwtTokenService> logger)
    {
        _logger = logger;
    }

    public string? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        // Try multiple claim types in order of preference
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? user.FindFirstValue("sub") 
                     ?? user.FindFirstValue("nameid")
                     ?? user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
        
        return string.IsNullOrEmpty(userId) ? null : userId;
    }

    public string? GetUserRoleFromClaims(ClaimsPrincipal user)
    {
        // Try multiple claim types in order of preference
        var userRole = user.FindFirstValue(ClaimTypes.Role)
                       ?? user.FindFirstValue("role")
                       ?? user.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        
        return string.IsNullOrEmpty(userRole) ? null : userRole;
    }

    public string? GetUserIdFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            
            // Validate token can be read (without signature verification for claim extraction)
            if (!handler.CanReadToken(token))
            {
                _logger.LogWarning("Token cannot be read");
                return null;
            }

            var jsonToken = handler.ReadJwtToken(token);
            
            // Extract userId from token claims
            var userId = jsonToken.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.NameIdentifier || 
                c.Type == "sub" || 
                c.Type == "nameid" ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            
            return string.IsNullOrEmpty(userId) ? null : userId;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract userId from token");
            return null;
        }
    }

    public string? GetUserRoleFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var handler = new JwtSecurityTokenHandler();
            
            // Validate token can be read
            if (!handler.CanReadToken(token))
            {
                return null;
            }

            var jsonToken = handler.ReadJwtToken(token);
            
            // Extract role from token claims
            var userRole = jsonToken.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.Role || 
                c.Type == "role" ||
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            
            return string.IsNullOrEmpty(userRole) ? null : userRole;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to extract userRole from token");
            return null;
        }
    }
}

