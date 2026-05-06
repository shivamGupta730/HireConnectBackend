using System.ComponentModel.DataAnnotations;

namespace HireConnect.Shared.Models;

public class Job
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Category { get; set; } = string.Empty;
    [Required]
    public JobType Type { get; set; }
    [Required]
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    [Required]
    public decimal SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Currency { get; set; } = "USD";
    public string[] Skills { get; set; } = Array.Empty<string>();
    public string? ExperienceRequired { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public int PostedBy { get; set; }
    public User? PostedByUser { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Active;
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ViewCount { get; set; } = 0;
    public int ApplicationCount { get; set; } = 0;
}

public class CreateJobRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Category { get; set; } = string.Empty;
    [Required]
    public JobType Type { get; set; }
    [Required]
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    [Required]
    public decimal SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Currency { get; set; } = "USD";
    public string[] Skills { get; set; } = Array.Empty<string>();
    public string? ExperienceRequired { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UpdateJobRequest
{
    public string? Title { get; set; }
    public string? Category { get; set; }
    public JobType? Type { get; set; }
    public string? Location { get; set; }
    public bool? IsRemote { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string? Currency { get; set; }
    public List<string>? Skills { get; set; }
    public string? ExperienceRequired { get; set; }
    public string? Description { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public JobStatus? Status { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
