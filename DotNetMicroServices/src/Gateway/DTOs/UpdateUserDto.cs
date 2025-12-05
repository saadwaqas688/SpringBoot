using System.ComponentModel.DataAnnotations;

namespace Gateway.DTOs;

/// <summary>
/// DTO for updating an existing user.
/// All fields are optional - only provided fields will be updated.
/// This DTO validates input format at the Gateway layer before forwarding to UserAccountService.
/// </summary>
public class UpdateUserDto
{
    private string? _firstName;

    /// <summary>
    /// User's first name. Optional field with maximum length of 255 characters.
    /// Automatically trims whitespace before validation.
    /// </summary>
    [StringLength(255, ErrorMessage = "First name cannot exceed 255 characters")]
    public string? FirstName
    {
        get => _firstName;
        set => _firstName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private string? _lastName;

    /// <summary>
    /// User's last name. Optional field with maximum length of 255 characters.
    /// Automatically trims whitespace before validation.
    /// </summary>
    [StringLength(255, ErrorMessage = "Last name cannot exceed 255 characters")]
    public string? LastName
    {
        get => _lastName;
        set => _lastName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private string? _email;

    /// <summary>
    /// User's email address. Optional field, must be valid email format if provided, maximum 255 characters.
    /// Automatically trims whitespace and converts to lowercase before validation.
    /// </summary>
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email
    {
        get => _email;
        set => _email = string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
    }

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
    /// User's role. Optional field with maximum length of 50 characters.
    /// </summary>
    [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
    public string? Role { get; set; }

    /// <summary>
    /// User's status (active/inactive). Optional field with maximum length of 20 characters.
    /// </summary>
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
    public string? Status { get; set; }

    /// <summary>
    /// User's profile image URL. Optional field.
    /// </summary>
    public string? Image { get; set; }
}


