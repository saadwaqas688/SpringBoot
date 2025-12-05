using System.ComponentModel.DataAnnotations;

namespace Gateway.DTOs;

/// <summary>
/// DTO for creating a new user.
/// This DTO validates input format at the Gateway layer before forwarding to UserAccountService.
/// Business logic validation (e.g., email uniqueness) is handled in the service layer.
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
    [Required(ErrorMessage = "First name is required")]
    [StringLength(255, ErrorMessage = "First name cannot exceed 255 characters")]
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
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(255, ErrorMessage = "Last name cannot exceed 255 characters")]
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
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string Email
    {
        get => _email;
        set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
    }

    /// <summary>
    /// User's password. Required field with minimum length of 6 characters.
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// User's mobile phone number. Optional field with maximum length of 20 characters.
    /// </summary>
    [StringLength(20, ErrorMessage = "Mobile phone cannot exceed 20 characters")]
    public string? MobilePhone { get; set; }

    /// <summary>
    /// User's date of birth. Optional field.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// User's gender. Optional field with maximum length of 50 characters.
    /// </summary>
    [StringLength(50, ErrorMessage = "Gender cannot exceed 50 characters")]
    public string? Gender { get; set; }

    /// <summary>
    /// User's country. Optional field with maximum length of 100 characters.
    /// </summary>
    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }

    /// <summary>
    /// User's state/province. Optional field with maximum length of 100 characters.
    /// </summary>
    [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
    public string? State { get; set; }

    /// <summary>
    /// User's city. Optional field with maximum length of 100 characters.
    /// </summary>
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    /// <summary>
    /// User's postal/zip code. Optional field with maximum length of 20 characters.
    /// </summary>
    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// User's role. Optional field with default value "user", maximum length of 50 characters.
    /// </summary>
    [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
    public string Role { get; set; } = "user";

    /// <summary>
    /// User's status (active/inactive). Optional field with default value "active", maximum length of 20 characters.
    /// </summary>
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    public string Status { get; set; } = "active";

    /// <summary>
    /// User's profile image URL. Optional field.
    /// </summary>
    public string? Image { get; set; }
}


