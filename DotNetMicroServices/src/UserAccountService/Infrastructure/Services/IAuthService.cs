using UserAccountService.Models;

namespace UserAccountService.Infrastructure.Services;

public interface IAuthService
{
    string GenerateJwtToken(UserAccount user);
    bool VerifyPassword(string password, string passwordHash);
    string HashPassword(string password);
}

