using System.ComponentModel.DataAnnotations;

namespace HireConnect.Shared.Models;

public class Candidate
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Mobile { get; set; }

    public List<string> Skills { get; set; } = new();

    public string? Experience { get; set; }

    public int ExperienceYears { get; set; }

    public string? Education { get; set; }

    public string? ResumeUrl { get; set; }

    public string? PortfolioUrl { get; set; }

    public string? LinkedInUrl { get; set; }

    public string? GitHubUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
