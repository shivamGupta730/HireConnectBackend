using HireConnect.Shared.Models;

namespace HireConnect.ProfileService.Repositories;

public interface IProfileRepository
{
    // 🔥 Candidate Profile
    Task<CandidateProfile?> GetCandidateByUserIdAsync(int userId);
    Task<CandidateProfile> CreateCandidateAsync(CandidateProfile candidate);
    Task<CandidateProfile> UpdateCandidateAsync(CandidateProfile candidate);
    
    Task<bool> DeleteCandidateAsync(int userId);

    // 🔥 Recruiter Profile
    Task<RecruiterProfile?> GetRecruiterByUserIdAsync(int userId);
    Task<RecruiterProfile> CreateRecruiterAsync(RecruiterProfile recruiter);
    Task<RecruiterProfile> UpdateRecruiterAsync(RecruiterProfile recruiter);
    Task<bool> DeleteRecruiterAsync(int userId);
}