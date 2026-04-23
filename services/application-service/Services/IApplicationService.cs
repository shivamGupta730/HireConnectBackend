using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.DTOs;

namespace HireConnect.ApplicationService.Services;

public interface IApplicationService
{
    Task<ApplicationResponseDto?> GetByIdAsync(int id);
    Task<PagedResponse<ApplicationResponseDto>> GetApplicationsByCandidateAsync(int candidateId, BaseRequest request);
    Task<PagedResponse<ApplicationResponseDto>> GetApplicationsByJobAsync(int jobId, int recruiterId, BaseRequest request);
    Task<ApplicationResponseDto> CreateApplicationAsync(int candidateId, CreateApplicationDto request);
    Task<bool> UpdateApplicationStatusAsync(int id, int recruiterId, UpdateApplicationStatusDto request);
    Task<bool> DeleteApplicationAsync(int id, int candidateId);
}
