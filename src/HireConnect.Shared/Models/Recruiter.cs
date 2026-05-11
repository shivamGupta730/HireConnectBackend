using System.ComponentModel.DataAnnotations;

namespace HireConnect.Shared.Models;

public class Recruiter
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Designation { get; set; }

    [MaxLength(100)]
    public string? Industry { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    public string? Description { get; set; }

    [MaxLength(50)]
    public string? CompanySize { get; set; }

    [MaxLength(255)]
    public string? Headquarters { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
