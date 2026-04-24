using HireConnect.InterviewService.Data;
using HireConnect.InterviewService.Models;
using HireConnect.InterviewService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.InterviewService.Repositories;

public class InterviewRepository : IInterviewRepository
{
    private readonly InterviewDbContext _context;

    public InterviewRepository(InterviewDbContext context)
    {
        _context = context;
    }

    public async Task<Interview?> GetByIdAsync(int id)
    {
        return await _context.Interviews
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<PagedResponse<Interview>> GetByCandidateIdAsync(int candidateId, BaseRequest request)
    {
        var query = _context.Interviews
            .Where(i => i.CandidateId == candidateId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var interviews = await query
            .OrderByDescending(i => i.ScheduledAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Interview>
        {
            Data = interviews,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResponse<Interview>> GetByJobIdAsync(int jobId, BaseRequest request)
    {
        var query = _context.Interviews
            .Where(i => i.JobId == jobId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var interviews = await query
            .OrderByDescending(i => i.ScheduledAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Interview>
        {
            Data = interviews,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Interview> CreateAsync(Interview interview)
    {
        _context.Interviews.Add(interview);
        await _context.SaveChangesAsync();
        return interview;
    }

    public async Task<bool> UpdateStatusAsync(int id, InterviewStatus status, string? notes = null)
    {
        var interview = await _context.Interviews.FindAsync(id);
        if (interview == null)
            return false;

        interview.Status = status;
        interview.Notes = notes;
        interview.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var interview = await _context.Interviews.FindAsync(id);
        if (interview == null)
            return false;

        _context.Interviews.Remove(interview);
        await _context.SaveChangesAsync();
        return true;
    }
}
