using System.ComponentModel.DataAnnotations;

namespace UserAccountService.DTOs;

/// <summary>
/// DTO for updating an existing user.
/// All fields are optional - only provided fields will be updated.
/// Uses property setters for simple transformations (trim, lowercase) - the idiomatic .NET approach.
/// </summary>
public class UpdateUserDto
{
    private string? _firstName;

    [StringLength(255)]
    public string? FirstName
    {
        get => _firstName;
        set => _firstName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private string? _lastName;

    [StringLength(255)]
    public string? LastName
    {
        get => _lastName;
        set => _lastName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private string? _email;

    [EmailAddress]
    [StringLength(255)]
    public string? Email
    {
        get => _email;
        set => _email = string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
    }

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
    public string? Role { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }
}


