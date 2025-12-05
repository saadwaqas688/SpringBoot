using System.ComponentModel.DataAnnotations;

namespace Gateway.Application.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}


