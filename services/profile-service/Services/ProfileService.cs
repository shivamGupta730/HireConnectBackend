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

    public async Task<Candidate?> GetMyProfileAsync(int userId)
    {
        return await _repository.GetCandidateByUserIdAsync(userId);
    }

    public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
    {
        return await _repository.CreateCandidateAsync(candidate);
    }

    public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
    {
        return await _repository.UpdateCandidateAsync(candidate);
    }

    public async Task<bool> DeleteCandidateAsync(int userId)
    {
        return await _repository.DeleteCandidateAsync(userId);
    }

    // Recruiter Profile Methods
    public async Task<Recruiter?> GetMyRecruiterProfileAsync(int userId)
    {
        return await _repository.GetRecruiterByUserIdAsync(userId);
    }

    public async Task<Recruiter> CreateRecruiterAsync(Recruiter recruiter)
    {
        return await _repository.CreateRecruiterAsync(recruiter);
    }

    public async Task<Recruiter> UpdateRecruiterAsync(Recruiter recruiter)
    {
        return await _repository.UpdateRecruiterAsync(recruiter);
    }

    public async Task<bool> DeleteRecruiterAsync(int userId)
    {
        return await _repository.DeleteRecruiterAsync(userId);
    }
}