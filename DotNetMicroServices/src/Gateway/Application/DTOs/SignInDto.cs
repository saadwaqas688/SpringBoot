using System.ComponentModel.DataAnnotations;

namespace Gateway.Application.DTOs;

public class SignInDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

