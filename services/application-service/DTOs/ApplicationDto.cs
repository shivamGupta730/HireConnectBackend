using System.ComponentModel.DataAnnotations;
using HireConnect.ApplicationService.Models;

namespace HireConnect.ApplicationService.DTOs;

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

public class CreateApplicationDto
{
    [Required]
    public int JobId { get; set; }
    
    public string? CoverLetter { get; set; }
    
    public string? ResumeUrl { get; set; }
    
    [Range(0, 1000000, ErrorMessage = "Expected salary must be between 0 and 1000000")]
    public decimal? ExpectedSalary { get; set; }
}

public class UpdateApplicationStatusDto
{
    [Required]
    public ApplicationStatus Status { get; set; }
    
    public string? Notes { get; set; }
}

public class ApplicationResponseDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int CandidateId { get; set; }
    public ApplicationStatus Status { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public decimal? ExpectedSalary { get; set; }
    public string? Notes { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? StatusChangedAt { get; set; }
}
