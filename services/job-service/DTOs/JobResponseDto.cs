using HireConnect.JobService.Models;
using HireConnect.Shared.Models;
using System.ComponentModel.DataAnnotations;
using JobStatus = HireConnect.JobService.Models.JobStatus;

namespace HireConnect.JobService.DTOs;

public class JobResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public decimal SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Currency { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public int ExperienceRequired { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    // PostedBy maps to UserId from Auth Service JWT
    public int PostedBy { get; set; }
    public JobStatus Status { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    public int ApplicationCount { get; set; }
}


public class UpdateJobStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public JobStatus Status { get; set; }
}
