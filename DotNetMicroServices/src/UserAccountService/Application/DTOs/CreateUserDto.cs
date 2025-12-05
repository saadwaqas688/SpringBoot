using System.ComponentModel.DataAnnotations;

namespace UserAccountService.Application.DTOs;

/// <summary>
/// DTO for creating a new user.
/// Uses property setters for simple transformations (trim, lowercase) - the idiomatic .NET approach.
/// Model binders are available for complex transformations (ObjectId, JSON parsing) if needed.
/// </summary>
public class CreateUserDto
{
    private string _firstName = string.Empty;

    /// <summary>
    /// User's first name. Required field with maximum length of 255 characters.
    /// Automatically trims whitespace before validation.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string FirstName
    {
        get => _firstName;
        set => _firstName = value?.Trim() ?? string.Empty;
    }

    private string _lastName = string.Empty;

    /// <summary>
    /// User's last name. Required field with maximum length of 255 characters.
    /// Automatically trims whitespace before validation.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string LastName
    {
        get => _lastName;
        set => _lastName = value?.Trim() ?? string.Empty;
    }

    private string _email = string.Empty;

    /// <summary>
    /// User's email address. Required field, must be valid email format, maximum 255 characters.
    /// Automatically trims whitespace and converts to lowercase before validation.
    /// Note: Email uniqueness is validated in the service layer (business logic).
    /// </summary>
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

    [StringLength(20)]
    public string? MobilePhone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(50)]
    public string? Gender { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = "user";

    [StringLength(20)]
    public string Status { get; set; } = "active";

    public string? Image { get; set; }
}


