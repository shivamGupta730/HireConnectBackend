using System.ComponentModel.DataAnnotations;

namespace HireConnect.InterviewService.Models;

public class Interview
{
    public int Id { get; set; }
    
    [Required]
    public int ApplicationId { get; set; }
    
    [Required]
    public int JobId { get; set; }
    
    [Required]
    public int CandidateId { get; set; }
    
    [Required]
    public DateTime ScheduledAt { get; set; }
    
    [StringLength(500)]
    public string? MeetingLink { get; set; }
    
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    
    [StringLength(1000)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}

public enum InterviewStatus
{
    Scheduled = 1,
    Completed = 2,
    Selected = 3,
    Rejected = 4,
    Cancelled = 5
}
