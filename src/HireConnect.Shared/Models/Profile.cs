using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HireConnect.Shared.Models;

public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
}

public class CandidateProfile
{
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Phone]
    public string? Mobile { get; set; }
    
    // Store skills as string in database (comma-separated)
    public string Skills { get; set; } = string.Empty;
    
    // Helper property for working with skills as List<string>
    [NotMapped]
    public List<string> SkillsList
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Skills))
                return new List<string>();
            return Skills.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        }
        set
        {
            Skills = string.Join(",", value ?? new List<string>());
        }
    }
    
    public string? Experience { get; set; }
    public int ExperienceYears { get; set; }
    public string? ResumeUrl { get; set; }
    public string? Education { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    [NotMapped]
    public Address? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class RecruiterProfile
{
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? CompanySize { get; set; }
    public string? Headquarters { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public Address? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class CandidateProfileUpdateRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Phone]
    public string? Mobile { get; set; }
    public List<string> Skills { get; set; } = new();
    public string? Experience { get; set; }
    public string? ResumeUrl { get; set; }
    public string? Education { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public Address? Address { get; set; }
}

public class RecruiterProfileUpdateRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? CompanySize { get; set; }
    public string? Headquarters { get; set; }
    public Address? Address { get; set; }
}

// Simplified entities for profile schema
public class Candidate
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Mobile { get; set; }
    public List<string> Skills { get; set; } = new();
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public string? ResumeUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class Recruiter
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }
    public string? CompanySize { get; set; }
    public string? Headquarters { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

// DTOs for Profile API
public class ProfileResponseDto
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
    public CandidateProfileDto? CandidateProfile { get; set; }
    public RecruiterProfileDto? RecruiterProfile { get; set; }
}

public class CandidateProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RecruiterProfileDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateProfileRequestDto
{
    public CandidateProfileUpdateDto? CandidateProfile { get; set; }
    public RecruiterProfileUpdateDto? RecruiterProfile { get; set; }
}

public class CandidateProfileUpdateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
}

public class RecruiterProfileUpdateDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string? Designation { get; set; }
}

// Generic wrapper class for handling JSON request structure with nested request object
public class RequestWrapper<T>
{
    public T? Request { get; set; }
}

// Wrapper DTO for creating candidate profile (handles JSON structure with nested request)
public class CreateCandidateWrapperRequest
{
    public CreateCandidateRequest request { get; set; } = new();
}

// Request DTO for creating candidate profile
public class CreateCandidateRequest
{
    public string fullName { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string mobile { get; set; } = string.Empty;
    public string skills { get; set; } = string.Empty;
    public string experience { get; set; } = string.Empty;
    public string education { get; set; } = string.Empty;
    public string resumeUrl { get; set; } = string.Empty;
    public string portfolioUrl { get; set; } = string.Empty;
    public string linkedInUrl { get; set; } = string.Empty;
    public string gitHubUrl { get; set; } = string.Empty;
}

// Response DTO for candidate profile
public class CandidateResponseDto
{
    public int id { get; set; }
    public string fullName { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string mobile { get; set; } = string.Empty;
    public string skills { get; set; } = string.Empty;
    public string experience { get; set; } = string.Empty;
    public string education { get; set; } = string.Empty;
    public string resumeUrl { get; set; } = string.Empty;
}

// Wrapper DTO for creating recruiter profile (handles JSON structure with nested request)
public class CreateRecruiterWrapperRequest
{
    public CreateRecruiterRequest request { get; set; } = new();
}

// Request DTO for creating recruiter profile
public class CreateRecruiterRequest
{
    public string fullName { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string industry { get; set; } = string.Empty;
    public string website { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string companySize { get; set; } = string.Empty;
    public string headquarters { get; set; } = string.Empty;
}

// Response DTO for recruiter profile
public class RecruiterResponseDto
{
    public int id { get; set; }
    public string fullName { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string industry { get; set; } = string.Empty;
    public string website { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public string companySize { get; set; } = string.Empty;
    public string headquarters { get; set; } = string.Empty;
}
