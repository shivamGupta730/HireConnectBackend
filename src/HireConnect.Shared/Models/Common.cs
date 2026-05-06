namespace HireConnect.Shared.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class BaseRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class SearchRequest : BaseRequest
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? Type { get; set; }
}

public enum UserRole
{
    Candidate = 1,
    Recruiter = 2,
    Admin = 3
}

public enum ApplicationStatus
{
    Applied = 1,
    Shortlisted = 2,
    InterviewScheduled = 3,
    Offered = 4,
    Rejected = 5
}

public enum InterviewStatus
{
    Scheduled = 1,
    Confirmed = 2,
    Rescheduled = 3,
    Cancelled = 4,
    Completed = 5
}

public enum InterviewMode
{
    Video = 1,
    Phone = 2,
    InPerson = 3
}

public enum JobType
{
    FullTime = 1,
    PartTime = 2,
    Contract = 3,
    Internship = 4,
    Remote = 5
}

public enum JobStatus
{
    Active = 1,
    Inactive = 2,
    Closed = 3
}

public enum NotificationType
{
    ApplicationReceived = 1,
    ApplicationStatusChanged = 2,
    InterviewScheduled = 3,
    JobPosted = 4,
    System = 5
}

public enum SubscriptionType
{
    Basic = 1,
    Premium = 2,
    Enterprise = 3
}

public enum SubscriptionStatus
{
    Active = 1,
    Cancelled = 2,
    Expired = 3
}
