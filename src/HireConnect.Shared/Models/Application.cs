using System.ComponentModel.DataAnnotations;

namespace HireConnect.Shared.Models;

public class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public Job? Job { get; set; }
    public int CandidateId { get; set; }
    public User? Candidate { get; set; }
    public CandidateProfile? CandidateProfile { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public decimal? ExpectedSalary { get; set; }
    public string? Notes { get; set; }
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? StatusChangedAt { get; set; }
}

public class CreateApplicationRequest
{
    [Required]
    public int JobId { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeUrl { get; set; }
    public decimal? ExpectedSalary { get; set; }
    public string? Notes { get; set; }
}

public class UpdateApplicationStatusRequest
{
    [Required]
    public ApplicationStatus Status { get; set; }
    public string? Notes { get; set; }
}
