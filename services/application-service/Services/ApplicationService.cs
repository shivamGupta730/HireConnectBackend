using HireConnect.ApplicationService.Repositories;
using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.DTOs;
using System.Net.Http.Json;
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
        
        // Send notification after successful application creation
        await SendApplicationNotificationAsync(createdApplication);
        
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
            var jobServiceUrl = _configuration["ServiceUrls:JobService"];
            if (string.IsNullOrEmpty(jobServiceUrl))
            {
                _logger.LogError("JobService URL is not configured");
                return (false, false);
            }

            var response = await _httpClient.GetAsync($"{jobServiceUrl}/api/job/{jobId}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Job API returned non-success status code {StatusCode} for jobId {JobId} at {Url}", response.StatusCode, jobId, jobServiceUrl);
                return (false, false);
            }

            var content = await response.Content.ReadAsStringAsync();
            var jobResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            // Handle optional ApiResponse wrapper
            JsonElement data = jobResponse;
            if (jobResponse.TryGetProperty("data", out var d))
            {
                data = d;
            }
            else if (jobResponse.TryGetProperty("Data", out var dp))
            {
                data = dp;
            }
            
            // Check for "success" property in wrapper
            if (jobResponse.TryGetProperty("success", out var successProp) || jobResponse.TryGetProperty("Success", out successProp))
            {
                if (successProp.ValueKind == JsonValueKind.False)
                {
                    return (false, false);
                }
            }

            // Check both camelCase and PascalCase for "status"
            if (data.TryGetProperty("status", out var statusProp) || data.TryGetProperty("Status", out statusProp))
            {
                int statusValue = 0;
                if (statusProp.ValueKind == JsonValueKind.Number)
                {
                    statusValue = statusProp.GetInt32();
                }
                else if (statusProp.ValueKind == JsonValueKind.String)
                {
                    var statusString = statusProp.GetString();
                    if (statusString == "Active" || statusString == "1") statusValue = 1;
                    else if (int.TryParse(statusString, out var parsedStatus)) statusValue = parsedStatus;
                }

                return (true, statusValue == 1); // 1 = Active
            }
            
            // If we found an "id" property, the job exists even if status is missing (default to active for now if missing)
            if (data.TryGetProperty("id", out _) || data.TryGetProperty("Id", out _))
            {
                return (true, true);
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
            var jobServiceUrl = _configuration["ServiceUrls:JobService"];
            if (string.IsNullOrEmpty(jobServiceUrl))
            {
                _logger.LogError("JobService URL is not configured");
                return false;
            }

            var response = await _httpClient.GetAsync($"{jobServiceUrl}/api/job/{jobId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var jobResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            // Handle optional ApiResponse wrapper
            JsonElement data = jobResponse;
            if (jobResponse.TryGetProperty("data", out var d))
            {
                data = d;
            }
            else if (jobResponse.TryGetProperty("Data", out var dp))
            {
                data = dp;
            }

            // Check both camelCase and PascalCase for "postedBy"
            if (data.TryGetProperty("postedBy", out var postedByProp) || data.TryGetProperty("PostedBy", out postedByProp))
            {
                int postedByValue = 0;
                if (postedByProp.ValueKind == JsonValueKind.Number)
                {
                    postedByValue = postedByProp.GetInt32();
                }
                else if (postedByProp.ValueKind == JsonValueKind.String)
                {
                    int.TryParse(postedByProp.GetString(), out postedByValue);
                }

                return postedByValue == recruiterId;
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

    private async Task SendApplicationNotificationAsync(Application application)
    {
        try
        {
            _logger.LogInformation("Sending notification for user {UserId}", application.CandidateId);

            var notification = new
            {
                userId = application.CandidateId,
                type = "Application",
                message = "Application submitted successfully"
            };

            var notificationServiceUrl = _configuration["ServiceUrls:NotificationService"];
            if (string.IsNullOrEmpty(notificationServiceUrl))
            {
                _logger.LogError("NotificationService URL is not configured");
                return;
            }

            var response = await _httpClient.PostAsJsonAsync($"{notificationServiceUrl}/api/Notification", notification);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Notification API failed with status {Status} at {Url}", response.StatusCode, notificationServiceUrl);
            }
            else
            {
                _logger.LogInformation("Notification sent successfully for user {UserId}", application.CandidateId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification for user {UserId}", application.CandidateId);
        }
    }
}
