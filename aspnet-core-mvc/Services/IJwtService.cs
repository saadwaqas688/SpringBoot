namespace CourseManagementAPI.Services;

public interface IJwtService
{
    string GenerateToken(string userId, string email, string role);
    string GetUserIdFromToken(string token);
    string GetRoleFromToken(string token);
}

