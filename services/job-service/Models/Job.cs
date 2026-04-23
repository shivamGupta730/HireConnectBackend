using System.ComponentModel.DataAnnotations;

namespace HireConnect.JobService.Models;

public class Job
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    public bool IsRemote { get; set; }
    
    [Required]
    public decimal SalaryMin { get; set; }
    
    public decimal? SalaryMax { get; set; }
    
    public string Currency { get; set; } = "USD";
    
    public string[] Skills { get; set; } = Array.Empty<string>();
    
    public int ExperienceRequired { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public string? Requirements { get; set; }
    
    public string? Benefits { get; set; }
    
    // PostedBy maps to UserId from Auth Service JWT
    public int PostedBy { get; set; }
    
    public JobStatus Status { get; set; } = JobStatus.Active;
    
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public int ViewCount { get; set; } = 0;
    
    public int ApplicationCount { get; set; } = 0;
}

public enum JobStatus
{
    Active = 1,
    Inactive = 2,
    Closed = 3
}
