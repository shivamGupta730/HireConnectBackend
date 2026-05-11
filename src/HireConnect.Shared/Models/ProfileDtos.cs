namespace HireConnect.Shared.Models;

public class CandidateResponseDto
{
    public int id { get; set; }
    public string fullName { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? mobile { get; set; }
    public string skills { get; set; } = string.Empty;
    public string? experience { get; set; }
    public string? education { get; set; }
    public string? resumeUrl { get; set; }
}

public class RecruiterResponseDto
{
    public int id { get; set; }
    public string fullName { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string? industry { get; set; }
    public string? website { get; set; }
    public string? description { get; set; }
    public string? companySize { get; set; }
    public string? headquarters { get; set; }
}

public class CreateCandidateRequest
{
    public string fullName { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string? mobile { get; set; }
    public string? skills { get; set; }
    public string? experience { get; set; }
    public string? education { get; set; }
    public string? resumeUrl { get; set; }
    public string? portfolioUrl { get; set; }
    public string? linkedInUrl { get; set; }
    public string? gitHubUrl { get; set; }
}

public class CreateRecruiterRequest
{
    public string fullName { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string? industry { get; set; }
    public string? website { get; set; }
    public string? description { get; set; }
    public string? companySize { get; set; }
    public string? headquarters { get; set; }
}

public class RequestWrapper<T>
{
    public T? Request { get; set; }
}
