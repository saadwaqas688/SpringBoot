using System.ComponentModel.DataAnnotations;

namespace UserAccountService.DTOs;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}


