using CourseManagementAPI.Models;

namespace CourseManagementAPI.Services;

public interface IAuthService
{
    Task<User> SignupAsync(string email, string password, string firstName, string lastName, string role = "USER");
    Task<string> SigninAsync(string email, string password);
}



