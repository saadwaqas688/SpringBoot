using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class UpdateTodoDto
{
    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool? IsCompleted { get; set; }
}

