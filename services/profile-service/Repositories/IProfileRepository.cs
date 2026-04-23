using HireConnect.Shared.Models;

namespace HireConnect.ProfileService.Repositories;

public interface IProfileRepository
{
    // 🔥 Candidate Profile
    Task<Candidate?> GetCandidateByUserIdAsync(int userId);
    Task<Candidate> CreateCandidateAsync(Candidate candidate);
    Task<Candidate> UpdateCandidateAsync(Candidate candidate);
    
    Task<bool> DeleteCandidateAsync(int userId);

    // 🔥 Recruiter Profile
    Task<Recruiter?> GetRecruiterByUserIdAsync(int userId);
    Task<Recruiter> CreateRecruiterAsync(Recruiter recruiter);
    Task<Recruiter> UpdateRecruiterAsync(Recruiter recruiter);
    Task<bool> DeleteRecruiterAsync(int userId);
}