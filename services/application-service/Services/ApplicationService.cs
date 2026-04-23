using HireConnect.ApplicationService.Repositories;
using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.DTOs;
using System.Text.Json;

namespace HireConnect.ApplicationService.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ApplicationService> logger)
    {
        _applicationRepository = applicationRepository;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApplicationResponseDto?> GetByIdAsync(int id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null)
        {
            return null;
        }

        return MapToApplicationResponseDto(application);
    }

    public async Task<PagedResponse<ApplicationResponseDto>> GetApplicationsByCandidateAsync(int candidateId, BaseRequest request)
    {
        var applications = await _applicationRepository.GetApplicationsByCandidateAsync(candidateId, request);
        
        return new PagedResponse<ApplicationResponseDto>
        {
            Data = applications.Data.Select(MapToApplicationResponseDto).ToList(),
            Page = applications.Page,
            PageSize = applications.PageSize,
            TotalCount = applications.TotalCount
        };
    }

    public async Task<PagedResponse<ApplicationResponseDto>> GetApplicationsByJobAsync(int jobId, int recruiterId, BaseRequest request)
    {
        // Validate recruiter owns the job
        var jobOwnership = await ValidateJobOwnershipAsync(jobId, recruiterId);
        if (!jobOwnership)
        {
            throw new UnauthorizedAccessException("You are not authorized to view applications for this job");
        }

        var applications = await _applicationRepository.GetApplicationsByJobAsync(jobId, request);
        
        return new PagedResponse<ApplicationResponseDto>
        {
            Data = applications.Data.Select(MapToApplicationResponseDto).ToList(),
            Page = applications.Page,
            PageSize = applications.PageSize,
            TotalCount = applications.TotalCount
        };
    }

    public async Task<ApplicationResponseDto> CreateApplicationAsync(int candidateId, CreateApplicationDto request)
    {
        // Check if already applied
        var existingApplication = await _applicationRepository.GetByJobAndCandidateAsync(request.JobId, candidateId);
        if (existingApplication != null)
        {
            throw new InvalidOperationException("You have already applied for this job");
        }

        // Validate job exists and is active
        var jobValidation = await ValidateJobAsync(request.JobId);
        if (!jobValidation.Exists)
        {
            throw new InvalidOperationException("Job not found");
        }
        if (!jobValidation.IsActive)
        {
            throw new InvalidOperationException("Job is not open for applications");
        }

        var application = new Application
        {
            JobId = request.JobId,
            CandidateId = candidateId,
            CoverLetter = request.CoverLetter,
            ResumeUrl = request.ResumeUrl,
            ExpectedSalary = request.ExpectedSalary,
            Status = ApplicationStatus.Applied,
            AppliedAt = DateTime.UtcNow,
            StatusChangedAt = DateTime.UtcNow
        };

        var createdApplication = await _applicationRepository.CreateAsync(application);
        return MapToApplicationResponseDto(createdApplication);
    }

    public async Task<bool> UpdateApplicationStatusAsync(int id, int recruiterId, UpdateApplicationStatusDto request)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null)
        {
            throw new InvalidOperationException("Application not found");
        }

        // Validate recruiter owns the job
        var jobOwnership = await ValidateJobOwnershipAsync(application.JobId, recruiterId);
        if (!jobOwnership)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this application");
        }

        // Validate status transition
        if (!IsValidStatusTransition(application.Status, request.Status))
        {
            throw new InvalidOperationException($"Invalid status transition from {application.Status} to {request.Status}");
        }

        return await _applicationRepository.UpdateStatusAsync(id, request.Status, request.Notes);
    }

    public async Task<bool> DeleteApplicationAsync(int id, int candidateId)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null)
        {
            throw new InvalidOperationException("Application not found");
        }

        if (application.CandidateId != candidateId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this application");
        }

        return await _applicationRepository.DeleteAsync(id);
    }

    private ApplicationResponseDto MapToApplicationResponseDto(Application application)
    {
        return new ApplicationResponseDto
        {
            Id = application.Id,
            JobId = application.JobId,
            CandidateId = application.CandidateId,
            Status = application.Status,
            CoverLetter = application.CoverLetter,
            ResumeUrl = application.ResumeUrl,
            ExpectedSalary = application.ExpectedSalary,
            Notes = application.Notes,
            AppliedAt = application.AppliedAt,
            UpdatedAt = application.UpdatedAt,
            StatusChangedAt = application.StatusChangedAt
        };
    }

    private async Task<(bool Exists, bool IsActive)> ValidateJobAsync(int jobId)
    {
        try
        {
            var jobServiceUrl = _configuration["ServiceUrls:JobService"] ?? "http://localhost:5003";
            var response = await _httpClient.GetAsync($"{jobServiceUrl}/api/job/{jobId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return (false, false);
            }

            var content = await response.Content.ReadAsStringAsync();
            var jobResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            if (jobResponse.TryGetProperty("status", out var statusProperty))
            {
                var status = statusProperty.GetInt32();
                return (true, status == 1); // 1 = Active
            }
            
            return (false, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate job {JobId}", jobId);
            return (false, false);
        }
    }

    private async Task<bool> ValidateJobOwnershipAsync(int jobId, int recruiterId)
    {
        try
        {
            var jobServiceUrl = _configuration["ServiceUrls:JobService"] ?? "http://localhost:5003";
            var response = await _httpClient.GetAsync($"{jobServiceUrl}/api/job/{jobId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var jobResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            if (jobResponse.TryGetProperty("postedBy", out var postedByProperty))
            {
                var postedBy = postedByProperty.GetInt32();
                return postedBy == recruiterId;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate job ownership for job {JobId} and recruiter {RecruiterId}", jobId, recruiterId);
            return false;
        }
    }

    private bool IsValidStatusTransition(ApplicationStatus currentStatus, ApplicationStatus newStatus)
    {
        // Allowed transitions:
        // Applied -> Shortlisted
        // Shortlisted -> InterviewScheduled
        // InterviewScheduled -> Offered / Rejected
        
        return (currentStatus, newStatus) switch
        {
            (ApplicationStatus.Applied, ApplicationStatus.Shortlisted) => true,
            (ApplicationStatus.Shortlisted, ApplicationStatus.InterviewScheduled) => true,
            (ApplicationStatus.InterviewScheduled, ApplicationStatus.Offered) => true,
            (ApplicationStatus.InterviewScheduled, ApplicationStatus.Rejected) => true,
            _ => false
        };
    }
}
