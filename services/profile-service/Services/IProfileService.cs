using HireConnect.Shared.Models;

namespace HireConnect.ProfileService.Services;

public interface IProfileService
{
    // Candidate Profile Methods
    Task<CandidateProfile?> GetMyProfileAsync(int userId);
    Task<CandidateProfile> CreateCandidateAsync(CandidateProfile candidate);
    Task<CandidateProfile> UpdateCandidateAsync(CandidateProfile candidate);
    Task<bool> DeleteCandidateAsync(int userId);
    
    // Recruiter Profile Methods
    Task<RecruiterProfile?> GetMyRecruiterProfileAsync(int userId);
    Task<RecruiterProfile> CreateRecruiterAsync(RecruiterProfile recruiter);
    Task<RecruiterProfile> UpdateRecruiterAsync(RecruiterProfile recruiter);
    Task<bool> DeleteRecruiterAsync(int userId);
}