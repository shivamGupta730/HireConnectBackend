using HireConnect.JobService.Data;
using HireConnect.JobService.Models;
using HireConnect.JobService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.JobService.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobDbContext _context;

    public JobRepository(JobDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetByIdAsync(int id)
    {
        return await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<PagedResponse<Job>> GetOpenJobsAsync(BaseRequest request)
    {
        var query = _context.Jobs
            .Where(j => j.Status == JobStatus.Active)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Job>
        {
            Data = jobs,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResponse<Job>> SearchJobsAsync(JobSearchRequestDto request)
    {
        var query = _context.Jobs
            .Where(j => j.Status == JobStatus.Active)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Query))
        {
            query = query.Where(j => 
                j.Title.ToLower().Contains(request.Query.ToLower()) ||
                j.Description.ToLower().Contains(request.Query.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(j => j.Category.ToLower() == request.Category.ToLower());
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            query = query.Where(j => j.Location.ToLower().Contains(request.Location.ToLower()));
        }

        if (request.MinSalary.HasValue)
        {
            query = query.Where(j => j.SalaryMin >= request.MinSalary.Value);
        }

        if (request.MaxSalary.HasValue)
        {
            query = query.Where(j => j.SalaryMax <= request.MaxSalary.Value || !j.SalaryMax.HasValue);
        }

        if (!string.IsNullOrEmpty(request.Type))
        {
            query = query.Where(j => j.Type == request.Type);
        }

        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Job>
        {
            Data = jobs,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    // PostedBy maps to UserId from Auth Service JWT
    public async Task<PagedResponse<Job>> GetJobsByRecruiterAsync(int recruiterId, BaseRequest request)
    {
        var query = _context.Jobs
            .Where(j => j.PostedBy == recruiterId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var jobs = await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<Job>
        {
            Data = jobs,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Job> CreateAsync(Job job)
    {
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<Job> UpdateAsync(Job job)
    {
        job.UpdatedAt = DateTime.UtcNow;
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
        return job;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
            return false;

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, JobStatus status)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
            return false;

        job.Status = status;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateViewCountAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
            return false;

        job.ViewCount++;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateApplicationCountAsync(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
            return false;

        job.ApplicationCount++;
        await _context.SaveChangesAsync();
        return true;
    }
}
