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
    public async Task<Candidate?> GetCandidateByUserIdAsync(int userId)
    {
        return await _context.Candidates
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Candidate> CreateCandidateAsync(Candidate candidate)
    {
        // Always use UserId as identity across services - check for existing profile
        var existingCandidate = await GetCandidateByUserIdAsync(candidate.UserId);
        if (existingCandidate != null)
        {
            throw new InvalidOperationException($"Candidate profile already exists for UserId: {candidate.UserId}");
        }

        _context.Candidates.Add(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
    {
        _context.Candidates.Update(candidate);
        await _context.SaveChangesAsync();
        return candidate;
    }

    public async Task<bool> DeleteCandidateAsync(int userId)
    {
        var candidate = await _context.Candidates
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (candidate == null)
            return false;

        _context.Candidates.Remove(candidate);
        await _context.SaveChangesAsync();
        return true;
    }

    // 🔥 Recruiter Profile
    public async Task<Recruiter?> GetRecruiterByUserIdAsync(int userId)
    {
        return await _context.Recruiters
            .FirstOrDefaultAsync(r => r.UserId == userId);
    }

    public async Task<Recruiter> CreateRecruiterAsync(Recruiter recruiter)
    {
        // Always use UserId as identity across services - check for existing profile
        var existingRecruiter = await GetRecruiterByUserIdAsync(recruiter.UserId);
        if (existingRecruiter != null)
        {
            throw new InvalidOperationException($"Recruiter profile already exists for UserId: {recruiter.UserId}");
        }

        _context.Recruiters.Add(recruiter);
        await _context.SaveChangesAsync();
        return recruiter;
    }

    public async Task<Recruiter> UpdateRecruiterAsync(Recruiter recruiter)
    {
        _context.Recruiters.Update(recruiter);
        await _context.SaveChangesAsync();
        return recruiter;
    }

    public async Task<bool> DeleteRecruiterAsync(int userId)
    {
        var recruiter = await _context.Recruiters
            .FirstOrDefaultAsync(r => r.UserId == userId);

        if (recruiter == null)
            return false;

        _context.Recruiters.Remove(recruiter);
        await _context.SaveChangesAsync();
        return true;
    }
}