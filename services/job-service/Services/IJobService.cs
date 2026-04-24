using HireConnect.Shared.Models;
using HireConnect.JobService.DTOs;
using HireConnect.JobService.Models;
using JobStatus = HireConnect.JobService.Models.JobStatus;

namespace HireConnect.JobService.Services;

public interface IJobService
{
    Task<JobResponseDto?> GetJobByIdAsync(int id);
    Task<PagedResponse<JobResponseDto>> GetOpenJobsAsync(BaseRequest request);
    Task<PagedResponse<JobResponseDto>> SearchJobsAsync(JobSearchRequestDto request);
    Task<PagedResponse<JobResponseDto>> GetJobsByRecruiterAsync(int recruiterId, BaseRequest request);
    Task<JobResponseDto> CreateJobAsync(int recruiterId, CreateJobDto request);
    Task<JobResponseDto?> UpdateJobAsync(int id, int recruiterId, UpdateJobDto request);
    Task<bool> DeleteJobAsync(int id, int recruiterId);
    Task<bool> UpdateJobStatusAsync(int id, int recruiterId, JobStatus status);
    Task<bool> IncrementViewCountAsync(int id);
    Task<bool> IncrementApplicationCountAsync(int id);
}
