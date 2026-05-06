using System.ComponentModel.DataAnnotations;

namespace HireConnect.Shared.Models;

public class Interview
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public Application? Application { get; set; }
    public DateTime ScheduledAt { get; set; }
    public InterviewMode Mode { get; set; }
    public string? MeetLink { get; set; }
    public string? Location { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public string? Notes { get; set; }
    public int ScheduledBy { get; set; }
    public User? ScheduledByUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class CreateInterviewRequest
{
    [Required]
    public int ApplicationId { get; set; }
    [Required]
    public DateTime ScheduledAt { get; set; }
    [Required]
    public InterviewMode Mode { get; set; }
    public string? MeetLink { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class UpdateInterviewRequest
{
    public DateTime? ScheduledAt { get; set; }
    public InterviewMode? Mode { get; set; }
    public string? MeetLink { get; set; }
    public string? Location { get; set; }
    public InterviewStatus? Status { get; set; }
    public string? Notes { get; set; }
}

public class RescheduleInterviewRequest
{
    [Required]
    public DateTime NewScheduledAt { get; set; }
    public string? Reason { get; set; }
}
