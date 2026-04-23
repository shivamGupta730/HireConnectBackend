using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.DTOs;

namespace HireConnect.ApplicationService.Repositories;

public interface IApplicationRepository
{
    Task<Application?> GetByIdAsync(int id);
    Task<Application?> GetByJobAndCandidateAsync(int jobId, int candidateId);
    Task<PagedResponse<Application>> GetApplicationsByCandidateAsync(int candidateId, BaseRequest request);
    Task<PagedResponse<Application>> GetApplicationsByJobAsync(int jobId, BaseRequest request);
    Task<Application> CreateAsync(Application application);
    Task<bool> UpdateAsync(Application application);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? notes = null);
}
