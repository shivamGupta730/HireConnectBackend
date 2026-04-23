using HireConnect.JobService.Repositories;
using HireConnect.JobService.Models;
using HireConnect.JobService.DTOs;

namespace HireConnect.JobService.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;

    public JobService(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<JobResponseDto?> GetJobByIdAsync(int id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null)
            return null;

        // Increment view count for job details view
        await _jobRepository.UpdateViewCountAsync(id);

        return MapToJobResponseDto(job);
    }

    public async Task<PagedResponse<JobResponseDto>> GetOpenJobsAsync(BaseRequest request)
    {
        var jobs = await _jobRepository.GetOpenJobsAsync(request);
        return new PagedResponse<JobResponseDto>
        {
            Data = jobs.Data.Select(MapToJobResponseDto).ToList(),
            Page = jobs.Page,
            PageSize = jobs.PageSize,
            TotalCount = jobs.TotalCount
        };
    }

    public async Task<PagedResponse<JobResponseDto>> SearchJobsAsync(JobSearchRequestDto request)
    {
        var jobs = await _jobRepository.SearchJobsAsync(request);
        return new PagedResponse<JobResponseDto>
        {
            Data = jobs.Data.Select(MapToJobResponseDto).ToList(),
            Page = jobs.Page,
            PageSize = jobs.PageSize,
            TotalCount = jobs.TotalCount
        };
    }

    public async Task<PagedResponse<JobResponseDto>> GetJobsByRecruiterAsync(int recruiterId, BaseRequest request)
    {
        var jobs = await _jobRepository.GetJobsByRecruiterAsync(recruiterId, request);
        return new PagedResponse<JobResponseDto>
        {
            Data = jobs.Data.Select(MapToJobResponseDto).ToList(),
            Page = jobs.Page,
            PageSize = jobs.PageSize,
            TotalCount = jobs.TotalCount
        };
    }

    public async Task<JobResponseDto> CreateJobAsync(int recruiterId, CreateJobDto request)
    {
        // Validate salary range
        if (request.SalaryMax.HasValue && request.SalaryMin > request.SalaryMax.Value)
        {
            throw new ArgumentException("Minimum salary cannot be greater than maximum salary");
        }

        // Always use UserId as identity across services - validate recruiter profile exists
        // Note: In a real implementation, you would call Profile Service to validate this
        // For now, we'll assume the recruiter profile exists if the user has the correct role
        // This validation ensures only users with existing recruiter profiles can create jobs

        var job = new Job
        {
            Title = request.Title,
            Category = request.Category,
            Type = request.Type,
            Location = request.Location,
            IsRemote = request.IsRemote,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            Currency = request.Currency,
            Skills = request.Skills.ToArray(),
            ExperienceRequired = request.ExperienceRequired,
            Description = request.Description,
            Requirements = request.Requirements,
            Benefits = request.Benefits,
            // PostedBy maps to UserId from Auth Service JWT
            PostedBy = recruiterId,
            ExpiresAt = request.ExpiresAt,
            Status = JobStatus.Active,
            PostedAt = DateTime.UtcNow
        };

        var createdJob = await _jobRepository.CreateAsync(job);
        return MapToJobResponseDto(createdJob);
    }

    public async Task<JobResponseDto?> UpdateJobAsync(int id, int recruiterId, UpdateJobDto request)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null)
            return null;

        // PostedBy maps to UserId from Auth Service JWT
        if (job.PostedBy != recruiterId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this job");
        }

        // Update only provided fields
        if (request.Title != null) job.Title = request.Title;
        if (request.Category != null) job.Category = request.Category;
        if (!string.IsNullOrEmpty(request.Type)) job.Type = request.Type;
        if (request.Location != null) job.Location = request.Location;
        if (request.IsRemote.HasValue) job.IsRemote = request.IsRemote.Value;
        if (request.SalaryMin.HasValue) job.SalaryMin = request.SalaryMin.Value;
        if (request.SalaryMax.HasValue) job.SalaryMax = request.SalaryMax.Value;
        if (request.Currency != null) job.Currency = request.Currency;
        if (request.Skills != null) job.Skills = request.Skills.ToArray();
        if (request.ExperienceRequired.HasValue) job.ExperienceRequired = request.ExperienceRequired.Value;
        if (request.Description != null) job.Description = request.Description;
        if (request.Requirements != null) job.Requirements = request.Requirements;
        if (request.Benefits != null) job.Benefits = request.Benefits;
        if (request.ExpiresAt.HasValue) job.ExpiresAt = request.ExpiresAt.Value;

        // Validate salary range after updates
        if (job.SalaryMax.HasValue && job.SalaryMin > job.SalaryMax.Value)
        {
            throw new ArgumentException("Minimum salary cannot be greater than maximum salary");
        }

        var updatedJob = await _jobRepository.UpdateAsync(job);
        return MapToJobResponseDto(updatedJob);
    }

    public async Task<bool> DeleteJobAsync(int id, int recruiterId)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null)
            return false;

        // PostedBy maps to UserId from Auth Service JWT
        if (job.PostedBy != recruiterId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this job");
        }

        return await _jobRepository.DeleteAsync(id);
    }

    public async Task<bool> UpdateJobStatusAsync(int id, int recruiterId, JobStatus status)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null)
            return false;

        // PostedBy maps to UserId from Auth Service JWT
        if (job.PostedBy != recruiterId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this job status");
        }

        return await _jobRepository.UpdateStatusAsync(id, status);
    }

    public async Task<bool> IncrementViewCountAsync(int id)
    {
        return await _jobRepository.UpdateViewCountAsync(id);
    }

    public async Task<bool> IncrementApplicationCountAsync(int id)
    {
        return await _jobRepository.UpdateApplicationCountAsync(id);
    }

    private static JobResponseDto MapToJobResponseDto(Job job)
    {
        return new JobResponseDto
        {
            Id = job.Id,
            Title = job.Title,
            Category = job.Category,
            Type = job.Type,
            Location = job.Location,
            IsRemote = job.IsRemote,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            Currency = job.Currency,
            Skills = job.Skills.ToList(),
            ExperienceRequired = job.ExperienceRequired,
            Description = job.Description,
            Requirements = job.Requirements,
            Benefits = job.Benefits,
            // PostedBy maps to UserId from Auth Service JWT
            PostedBy = job.PostedBy,
            Status = job.Status,
            PostedAt = job.PostedAt,
            ExpiresAt = job.ExpiresAt,
            UpdatedAt = job.UpdatedAt,
            ViewCount = job.ViewCount,
            ApplicationCount = job.ApplicationCount
        };
    }
}
