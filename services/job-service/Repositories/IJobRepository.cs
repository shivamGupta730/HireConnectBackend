using HireConnect.JobService.Models;
using HireConnect.JobService.DTOs;

namespace HireConnect.JobService.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(int id);
    Task<PagedResponse<Job>> GetOpenJobsAsync(BaseRequest request);
    Task<PagedResponse<Job>> SearchJobsAsync(JobSearchRequestDto request);
    Task<PagedResponse<Job>> GetJobsByRecruiterAsync(int recruiterId, BaseRequest request);
    Task<Job> CreateAsync(Job job);
    Task<Job> UpdateAsync(Job job);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStatusAsync(int id, JobStatus status);
    Task<bool> UpdateViewCountAsync(int id);
    Task<bool> UpdateApplicationCountAsync(int id);
}
