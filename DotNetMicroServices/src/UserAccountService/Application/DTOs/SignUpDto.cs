using System.ComponentModel.DataAnnotations;

namespace UserAccountService.Application.DTOs;

/// <summary>
/// DTO for user sign-up.
/// Uses property setters for simple transformations (trim, lowercase) - the idiomatic .NET approach.
/// </summary>
public class SignUpDto
{
    private string _name = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name
    {
        get => _name;
        set => _name = value?.Trim() ?? string.Empty;
    }

    private string _email = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email
    {
        get => _email;
        set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? Image { get; set; }
}

