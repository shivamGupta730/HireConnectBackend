using HireConnect.Shared.Models;

namespace HireConnect.ProfileService.Services;

public interface IProfileService
{
    // Candidate Profile Methods
    Task<Candidate?> GetMyProfileAsync(int userId);
    Task<Candidate> CreateCandidateAsync(Candidate candidate);
    Task<Candidate> UpdateCandidateAsync(Candidate candidate);
    Task<bool> DeleteCandidateAsync(int userId);
    
    // Recruiter Profile Methods
    Task<Recruiter?> GetMyRecruiterProfileAsync(int userId);
    Task<Recruiter> CreateRecruiterAsync(Recruiter recruiter);
    Task<Recruiter> UpdateRecruiterAsync(Recruiter recruiter);
    Task<bool> DeleteRecruiterAsync(int userId);
}