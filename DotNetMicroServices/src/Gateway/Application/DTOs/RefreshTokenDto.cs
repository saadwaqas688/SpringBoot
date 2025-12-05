using System.ComponentModel.DataAnnotations;

namespace Gateway.Application.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
}


