using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class CreateTodoDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }
}

