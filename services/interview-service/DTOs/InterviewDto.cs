using System.ComponentModel.DataAnnotations;
using HireConnect.InterviewService.Models;

namespace HireConnect.InterviewService.DTOs;

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

public class ScheduleInterviewDto
{
    [Required]
    public int ApplicationId { get; set; }
    
    [Required]
    public DateTime ScheduledAt { get; set; }
    
    [StringLength(500, ErrorMessage = "Meeting link cannot exceed 500 characters")]
    public string? MeetingLink { get; set; }
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}

public class UpdateInterviewStatusDto
{
    [Required]
    public InterviewStatus Status { get; set; }
    
    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}

public class InterviewResponseDto
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? MeetingLink { get; set; }
    public InterviewStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Additional info from Application Service
    public int JobId { get; set; }
    public int CandidateId { get; set; }
}
