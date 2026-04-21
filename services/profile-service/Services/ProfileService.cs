using HireConnect.ProfileService.Repositories;
using HireConnect.Shared.Models;

namespace HireConnect.ProfileService.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _repository;

    public ProfileService(IProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<CandidateProfile?> GetMyProfileAsync(int userId)
    {
        return await _repository.GetCandidateByUserIdAsync(userId);
    }

    public async Task<CandidateProfile> CreateCandidateAsync(CandidateProfile candidate)
    {
        return await _repository.CreateCandidateAsync(candidate);
    }

    public async Task<CandidateProfile> UpdateCandidateAsync(CandidateProfile candidate)
    {
        return await _repository.UpdateCandidateAsync(candidate);
    }

    public async Task<bool> DeleteCandidateAsync(int userId)
    {
        return await _repository.DeleteCandidateAsync(userId);
    }

    // Recruiter Profile Methods
    public async Task<RecruiterProfile?> GetMyRecruiterProfileAsync(int userId)
    {
        return await _repository.GetRecruiterByUserIdAsync(userId);
    }

    public async Task<RecruiterProfile> CreateRecruiterAsync(RecruiterProfile recruiter)
    {
        return await _repository.CreateRecruiterAsync(recruiter);
    }

    public async Task<RecruiterProfile> UpdateRecruiterAsync(RecruiterProfile recruiter)
    {
        return await _repository.UpdateRecruiterAsync(recruiter);
    }

    public async Task<bool> DeleteRecruiterAsync(int userId)
    {
        return await _repository.DeleteRecruiterAsync(userId);
    }
}