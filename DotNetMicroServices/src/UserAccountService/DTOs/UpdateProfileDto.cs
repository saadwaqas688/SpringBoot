using System.ComponentModel.DataAnnotations;

namespace UserAccountService.DTOs;

public class UpdateProfileDto
{
    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }
    
    // Optional profile fields
    [StringLength(50)]
    public string? Gender { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [StringLength(20)]
    public string? MobilePhone { get; set; }
    
    [StringLength(100)]
    public string? Country { get; set; }
    
    [StringLength(100)]
    public string? State { get; set; }
    
    [StringLength(100)]
    public string? City { get; set; }
    
    [StringLength(20)]
    public string? PostalCode { get; set; }
}

