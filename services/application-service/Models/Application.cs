using System.ComponentModel.DataAnnotations;

namespace HireConnect.ApplicationService.Models;

public class Application
{
    public int Id { get; set; }
    
    [Required]
    public int JobId { get; set; }
    
    [Required]
    public int CandidateId { get; set; }
    
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    
    public string? CoverLetter { get; set; }
    
    public string? ResumeUrl { get; set; }
    
    public decimal? ExpectedSalary { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? StatusChangedAt { get; set; }
}

public enum ApplicationStatus
{
    Applied = 1,
    Shortlisted = 2,
    InterviewScheduled = 3,
    Offered = 4,
    Rejected = 5
}
