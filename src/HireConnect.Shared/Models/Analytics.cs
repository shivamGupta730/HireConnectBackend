namespace HireConnect.Shared.Models;

public class AnalyticsSummary
{
    public int Id { get; set; }
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public int ShortlistedCount { get; set; }
    public int OfferedCount { get; set; }
    public int RejectedCount { get; set; }
    public decimal ViewToApplyRatio { get; set; }
    public decimal AverageTimeToHire { get; set; } // in days
    public int ActiveJobs { get; set; }
    public int ClosedJobs { get; set; }
    public int TotalCandidates { get; set; }
    public int TotalRecruiters { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class JobAnalytics
{
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int ApplicationCount { get; set; }
    public decimal ViewToApplyRatio { get; set; }
    public List<ApplicationStatusCount> StatusBreakdown { get; set; } = new();
}

public class ApplicationStatusCount
{
    public ApplicationStatus Status { get; set; }
    public int Count { get; set; }
}

public class RecruiterAnalytics
{
    public int RecruiterId { get; set; }
    public string RecruiterName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int JobsPosted { get; set; }
    public int ApplicationsReceived { get; set; }
    public decimal AverageTimeToHire { get; set; }
    public int HiresMade { get; set; }
}
