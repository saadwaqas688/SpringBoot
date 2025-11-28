using System.ComponentModel.DataAnnotations;

namespace UserAccountService.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}


