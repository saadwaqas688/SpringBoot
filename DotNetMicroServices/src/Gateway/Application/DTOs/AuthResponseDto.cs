namespace Gateway.Application.DTOs;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserInfoDto User { get; set; } = new();
}

public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Image { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public DateTime? CreatedAt { get; set; }
    
    // Optional profile fields
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? MobilePhone { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
}

