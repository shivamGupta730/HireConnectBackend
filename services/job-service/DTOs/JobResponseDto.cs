using HireConnect.JobService.Models;
using System.ComponentModel.DataAnnotations;

namespace HireConnect.JobService.DTOs;

public class BaseRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

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

public class JobSearchRequestDto : BaseRequest
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? Type { get; set; }
}

public class UpdateJobStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public JobStatus Status { get; set; }
}
