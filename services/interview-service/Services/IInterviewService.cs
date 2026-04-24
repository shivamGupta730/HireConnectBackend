using HireConnect.InterviewService.Models;
using HireConnect.InterviewService.DTOs;

namespace HireConnect.InterviewService.Services;

public interface IInterviewService
{
    Task<InterviewResponseDto?> GetByIdAsync(int id);
    Task<PagedResponse<InterviewResponseDto>> GetMyInterviewsAsync(int candidateId, BaseRequest request);
    Task<PagedResponse<InterviewResponseDto>> GetInterviewsByJobAsync(int jobId, int recruiterId, BaseRequest request);
    Task<InterviewResponseDto> ScheduleInterviewAsync(int recruiterId, ScheduleInterviewDto request);
    Task<bool> UpdateInterviewStatusAsync(int id, int recruiterId, UpdateInterviewStatusDto request);
    Task<bool> CancelInterviewAsync(int id, int recruiterId);
}
