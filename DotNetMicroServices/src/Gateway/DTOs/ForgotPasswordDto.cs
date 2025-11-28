using System.ComponentModel.DataAnnotations;

namespace Gateway.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}


