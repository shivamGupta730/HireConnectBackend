using HireConnect.ProfileService.Data;
using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.ProfileService.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileDbContext _context;

    public ProfileRepository(ProfileDbContext context)
    {
        _context = context;
    }

    // 🔥 Candidate Profile
    public async Task<CandidateProfile?> GetCandidateByUserIdAsync(int userId)
    {
        return await _context.CandidateProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CandidateProfile> CreateCandidateAsync(CandidateProfile candidate)
    {
        _context.CandidateProfiles.Add(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    public async Task<CandidateProfile> UpdateCandidateAsync(CandidateProfile candidate)
    {
        _context.CandidateProfiles.Update(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    public async Task<bool> DeleteCandidateAsync(int userId)
    {
        var candidate = await _context.CandidateProfiles
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (candidate == null)
            return false;

        _context.CandidateProfiles.Remove(candidate);
        await _context.SaveChangesAsync();
        return true;
    }

    // 🔥 Recruiter Profile
    public async Task<RecruiterProfile?> GetRecruiterByUserIdAsync(int userId)
    {
        return await _context.RecruiterProfiles
            .FirstOrDefaultAsync(r => r.UserId == userId);
    }

    public async Task<RecruiterProfile> CreateRecruiterAsync(RecruiterProfile recruiter)
    {
        _context.RecruiterProfiles.Add(recruiter);
        await _context.SaveChangesAsync();
        return recruiter;
    }

    public async Task<RecruiterProfile> UpdateRecruiterAsync(RecruiterProfile recruiter)
    {
        _context.RecruiterProfiles.Update(recruiter);
        await _context.SaveChangesAsync();
        return recruiter;
    }

    public async Task<bool> DeleteRecruiterAsync(int userId)
    {
        var recruiter = await _context.RecruiterProfiles
            .FirstOrDefaultAsync(r => r.UserId == userId);

        if (recruiter == null)
            return false;

        _context.RecruiterProfiles.Remove(recruiter);
        await _context.SaveChangesAsync();
        return true;
    }
}