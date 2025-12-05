using System.ComponentModel.DataAnnotations;

namespace UserAccountService.Application.DTOs;

public class SignInDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

