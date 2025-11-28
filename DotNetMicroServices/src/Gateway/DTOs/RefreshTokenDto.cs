using System.ComponentModel.DataAnnotations;

namespace Gateway.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}


