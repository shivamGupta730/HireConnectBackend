using HireConnect.InterviewService.Models;
using HireConnect.InterviewService.DTOs;

namespace HireConnect.InterviewService.Repositories;

public interface IInterviewRepository
{
    Task<Interview?> GetByIdAsync(int id);
    Task<PagedResponse<Interview>> GetByCandidateIdAsync(int candidateId, BaseRequest request);
    Task<PagedResponse<Interview>> GetByJobIdAsync(int jobId, BaseRequest request);
    Task<Interview> CreateAsync(Interview interview);
    Task<bool> UpdateStatusAsync(int id, InterviewStatus status, string? notes = null);
    Task<bool> DeleteAsync(int id);
}
