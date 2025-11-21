using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs;

public class UpdateUserDto
{
    [StringLength(50)]
    public string? Username { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }
}

