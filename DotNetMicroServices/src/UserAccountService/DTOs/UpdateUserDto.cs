using System.ComponentModel.DataAnnotations;

namespace UserAccountService.DTOs;

public class UpdateUserDto
{
    [StringLength(255)]
    public string? FirstName { get; set; }

    [StringLength(255)]
    public string? LastName { get; set; }

    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

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


