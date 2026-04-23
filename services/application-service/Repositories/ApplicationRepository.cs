using HireConnect.ApplicationService.Data;
using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.ApplicationService.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Application?> GetByIdAsync(int id)
    {
        return await _context.Applications
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Application?> GetByJobAndCandidateAsync(int jobId, int candidateId)
    {
        return await _context.Applications
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.CandidateId == candidateId);
    }

    public async Task<PagedResponse<Application>> GetApplicationsByCandidateAsync(int candidateId, BaseRequest request)
    {
        var query = _context.Applications
            .Where(a => a.CandidateId == candidateId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Application>
        {
            Data = applications,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResponse<Application>> GetApplicationsByJobAsync(int jobId, BaseRequest request)
    {
        var query = _context.Applications
            .Where(a => a.JobId == jobId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Application>
        {
            Data = applications,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Application> CreateAsync(Application application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<bool> UpdateAsync(Application application)
    {
        application.UpdatedAt = DateTime.UtcNow;
        application.StatusChangedAt = DateTime.UtcNow;
        _context.Applications.Update(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return false;

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, ApplicationStatus status, string? notes = null)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
            return false;

        application.Status = status;
        application.Notes = notes;
        application.UpdatedAt = DateTime.UtcNow;
        application.StatusChangedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
