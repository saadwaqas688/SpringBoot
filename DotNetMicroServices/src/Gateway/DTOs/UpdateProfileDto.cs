using System.ComponentModel.DataAnnotations;

namespace Gateway.DTOs;

public class UpdateProfileDto
{
    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(255)]
    public string? Image { get; set; }
}

