using HireConnect.InterviewService.Repositories;
using HireConnect.InterviewService.Models;
using HireConnect.InterviewService.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace HireConnect.InterviewService.Services;

public class InterviewService : IInterviewService
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InterviewService> _logger;

    public InterviewService(
        IInterviewRepository interviewRepository,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<InterviewService> logger)
    {
        _interviewRepository = interviewRepository;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<InterviewResponseDto?> GetByIdAsync(int id)
    {
        var interview = await _interviewRepository.GetByIdAsync(id);
        if (interview == null)
        {
            return null;
        }

        return MapToInterviewResponseDto(interview);
    }

    public async Task<PagedResponse<InterviewResponseDto>> GetMyInterviewsAsync(int candidateId, BaseRequest request)
    {
        var allInterviews = await _interviewRepository.GetByCandidateIdAsync(candidateId, request);
        
        var interviewResponses = new List<InterviewResponseDto>();
        foreach (var interview in allInterviews.Data)
        {
            interviewResponses.Add(MapToInterviewResponseDto(interview));
        }

        return new PagedResponse<InterviewResponseDto>
        {
            Data = interviewResponses,
            Page = allInterviews.Page,
            PageSize = allInterviews.PageSize,
            TotalCount = allInterviews.TotalCount
        };
    }

    public async Task<PagedResponse<InterviewResponseDto>> GetInterviewsByJobAsync(int jobId, int recruiterId, BaseRequest request)
    {
        // Validate recruiter owns the job
        var jobOwnership = await ValidateJobOwnershipAsync(jobId, recruiterId);
        if (!jobOwnership)
        {
            throw new UnauthorizedAccessException("You are not authorized to view interviews for this job");
        }

        var allInterviews = await _interviewRepository.GetByJobIdAsync(jobId, request);
        
        var interviewResponses = new List<InterviewResponseDto>();
        foreach (var interview in allInterviews.Data)
        {
            interviewResponses.Add(MapToInterviewResponseDto(interview));
        }

        return new PagedResponse<InterviewResponseDto>
        {
            Data = interviewResponses,
            Page = allInterviews.Page,
            PageSize = allInterviews.PageSize,
            TotalCount = allInterviews.TotalCount
        };
    }

    public async Task<InterviewResponseDto> ScheduleInterviewAsync(int recruiterId, ScheduleInterviewDto request)
    {
        // Validate application exists and recruiter owns it
        var applicationInfo = await ValidateApplicationAsync(request.ApplicationId, recruiterId);
        if (applicationInfo == null)
        {
            throw new InvalidOperationException("Application not found or you are not authorized");
        }

        // Check if interview is already scheduled for this application
        var existingInterviews = await GetInterviewsByApplicationAsync(request.ApplicationId);
        if (existingInterviews.Any(i => i.Status == InterviewStatus.Scheduled))
        {
            throw new InvalidOperationException("Interview is already scheduled for this application");
        }

        var interview = new Interview
        {
            ApplicationId = request.ApplicationId,
            JobId = applicationInfo.JobId,
            CandidateId = applicationInfo.CandidateId,
            ScheduledAt = request.ScheduledAt.ToUniversalTime(),
            MeetingLink = request.MeetingLink,
            Notes = request.Notes,
            Status = InterviewStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        var createdInterview = await _interviewRepository.CreateAsync(interview);
        
        // Update application status to InterviewScheduled
        await UpdateApplicationStatusAsync(request.ApplicationId, "InterviewScheduled");
        
        // Send notification after successful interview creation
        await SendInterviewNotificationAsync(createdInterview);

        return MapToInterviewResponseDto(createdInterview);
    }

    public async Task<bool> UpdateInterviewStatusAsync(int id, int recruiterId, UpdateInterviewStatusDto request)
    {
        var interview = await _interviewRepository.GetByIdAsync(id);
        if (interview == null)
        {
            throw new InvalidOperationException("Interview not found");
        }

        // Validate recruiter owns the job
        var jobOwnership = await ValidateJobOwnershipAsync(interview.JobId, recruiterId);
        if (!jobOwnership)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this interview");
        }

        // Validate status transition
        if (!IsValidStatusTransition(interview.Status, request.Status))
        {
            throw new InvalidOperationException($"Invalid status transition from {interview.Status} to {request.Status}");
        }

        return await _interviewRepository.UpdateStatusAsync(id, request.Status, request.Notes);
    }

    public async Task<bool> CancelInterviewAsync(int id, int recruiterId)
    {
        var interview = await _interviewRepository.GetByIdAsync(id);
        if (interview == null)
        {
            throw new InvalidOperationException("Interview not found");
        }

        // Validate recruiter owns the job
        var jobOwnership = await ValidateJobOwnershipAsync(interview.JobId, recruiterId);
        if (!jobOwnership)
        {
            throw new UnauthorizedAccessException("You are not authorized to cancel this interview");
        }

        var result = await _interviewRepository.UpdateStatusAsync(id, InterviewStatus.Cancelled, "Interview cancelled by recruiter");
        
        // Update application status back to Shortlisted
        await UpdateApplicationStatusAsync(interview.ApplicationId, "Shortlisted");
        
        return result;
    }

    private async Task<ApplicationInfo?> GetApplicationInfoAsync(int applicationId)
    {
        try
        {
            var applicationServiceUrl = _configuration["ServiceUrls:ApplicationService"] ?? "http://localhost:5004";
            var response = await _httpClient.GetAsync($"{applicationServiceUrl}/api/application/{applicationId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var applicationResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            if (applicationResponse.TryGetProperty("jobId", out var jobIdProperty) &&
                applicationResponse.TryGetProperty("candidateId", out var candidateIdProperty))
            {
                return new ApplicationInfo
                {
                    JobId = jobIdProperty.GetInt32(),
                    CandidateId = candidateIdProperty.GetInt32()
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get application info for {ApplicationId}", applicationId);
            return null;
        }
    }

    private async Task<ApplicationInfo?> ValidateApplicationAsync(int applicationId, int recruiterId)
    {
        try
        {
            var applicationServiceUrl = _configuration["ServiceUrls:ApplicationService"] ?? "http://localhost:5004";
            var response = await _httpClient.GetAsync($"{applicationServiceUrl}/api/application/{applicationId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var applicationResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            if (!applicationResponse.TryGetProperty("jobId", out var jobIdProperty) ||
                !applicationResponse.TryGetProperty("candidateId", out var candidateIdProperty))
            {
                return null;
            }

            var jobId = jobIdProperty.GetInt32();
            
            // Validate recruiter owns the job
            var jobOwnership = await ValidateJobOwnershipAsync(jobId, recruiterId);
            if (!jobOwnership)
            {
                return null;
            }

            return new ApplicationInfo
            {
                JobId = jobId,
                CandidateId = candidateIdProperty.GetInt32()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate application {ApplicationId}", applicationId);
            return null;
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

    private async Task UpdateApplicationStatusAsync(int applicationId, string status)
    {
        try
        {
            var applicationServiceUrl = _configuration["ServiceUrls:ApplicationService"] ?? "http://localhost:5004";
            var updateRequest = new { Status = status };
            var content = JsonSerializer.Serialize(updateRequest);
            
            await _httpClient.PutAsync(
                $"{applicationServiceUrl}/api/application/{applicationId}",
                new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update application status for {ApplicationId}", applicationId);
        }
    }

    private async Task<List<Interview>> GetInterviewsByApplicationAsync(int applicationId)
    {
        var interviews = new List<Interview>();
        // This is a simplified implementation - in production, you'd have a proper repository method
        var interview = await _interviewRepository.GetByIdAsync(applicationId);
        if (interview != null)
        {
            interviews.Add(interview);
        }
        return interviews;
    }

    private bool IsValidStatusTransition(InterviewStatus currentStatus, InterviewStatus newStatus)
    {
        // Allowed transitions:
        // Scheduled -> Completed
        // Completed -> Selected / Rejected
        // Any -> Cancelled
        
        return (currentStatus, newStatus) switch
        {
            (InterviewStatus.Scheduled, InterviewStatus.Completed) => true,
            (InterviewStatus.Completed, InterviewStatus.Selected) => true,
            (InterviewStatus.Completed, InterviewStatus.Rejected) => true,
            (_, InterviewStatus.Cancelled) => true,
            _ => false
        };
    }

    private InterviewResponseDto MapToInterviewResponseDto(Interview interview)
    {
        return new InterviewResponseDto
        {
            Id = interview.Id,
            ApplicationId = interview.ApplicationId,
            JobId = interview.JobId,
            CandidateId = interview.CandidateId,
            ScheduledAt = interview.ScheduledAt,
            MeetingLink = interview.MeetingLink,
            Status = interview.Status,
            Notes = interview.Notes,
            CreatedAt = interview.CreatedAt,
            UpdatedAt = interview.UpdatedAt
        };
    }

    private async Task SendInterviewNotificationAsync(Interview interview)
    {
        try
        {
            var notificationServiceUrl = _configuration["ServiceUrls:NotificationService"] ?? "http://localhost:5006";
            
            _logger.LogInformation(
                "Sending interview notification to user {UserId} via {Url}",
                interview.CandidateId,
                notificationServiceUrl
            );

            await _httpClient.PostAsJsonAsync($"{notificationServiceUrl}/api/Notification", new
            {
                userId = interview.CandidateId,
                type = "Interview",
                message = "Interview scheduled successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification failed but interview should still succeed");
        }
    }
}

public class ApplicationInfo
{
    public int JobId { get; set; }
    public int CandidateId { get; set; }
}
