using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Repositories;
using DotNetMessaging.API.Hubs;
using BCrypt.Net;

namespace DotNetMessaging.API.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    string GenerateJwtToken(User user);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<ChatHub> _hubContext;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, IHubContext<ChatHub> hubContext)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _hubContext = hubContext;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.GetByEmailAsync(request.Email) != null ||
            await _userRepository.GetByUsernameAsync(request.Username) != null)
        {
            return null;
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            User = MapToDto(user)
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        user.IsOnline = true;
        user.LastSeen = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user.Id, user);

        // Notify all connected clients in real-time that this user is now online
        await _hubContext.Clients.All.SendAsync("UserOnline", user.Id);

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            User = MapToDto(user)
        };
    }

    public string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Status = user.Status,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen
        };
    }
}
