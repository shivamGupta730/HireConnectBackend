using FluentAssertions;
using HireConnect.ProfileService.Data;
using HireConnect.ProfileService.Repositories;
using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.ProfileService.Tests.Repositories;

[TestClass]
public class ProfileRepositoryTests
{
    private ProfileDbContext _context = null!;
    private ProfileRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ProfileDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ProfileDbContext(options);
        _repository = new ProfileRepository(_context);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    [TestMethod]
    public async Task CreateCandidateAsync_ShouldCreateCandidate()
    {
        var candidate = new Candidate { UserId = 1, FullName = "Test", Email = "test@test.com", Skills = new List<string> { "C#" } };
        var result = await _repository.CreateCandidateAsync(candidate);
        result.Id.Should().BeGreaterThan(0);
        result.FullName.Should().Be("Test");
    }

    [TestMethod]
    public async Task GetCandidateByUserIdAsync_ShouldReturnCandidate_WhenExists()
    {
        var candidate = new Candidate { UserId = 1, FullName = "Test", Email = "test@test.com", Skills = new List<string>() };
        await _repository.CreateCandidateAsync(candidate);
        var result = await _repository.GetCandidateByUserIdAsync(1);
        result.Should().NotBeNull();
        result!.UserId.Should().Be(1);
    }

    [TestMethod]
    public async Task GetCandidateByUserIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetCandidateByUserIdAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteCandidateAsync_ShouldReturnTrue_WhenExists()
    {
        var candidate = new Candidate { UserId = 1, FullName = "Test", Email = "test@test.com", Skills = new List<string>() };
        await _repository.CreateCandidateAsync(candidate);
        var result = await _repository.DeleteCandidateAsync(1);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteCandidateAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.DeleteCandidateAsync(999);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task CreateRecruiterAsync_ShouldCreateRecruiter()
    {
        var recruiter = new Recruiter { UserId = 1, FullName = "Recruiter", CompanyName = "TestCo" };
        var result = await _repository.CreateRecruiterAsync(recruiter);
        result.Id.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public async Task GetRecruiterByUserIdAsync_ShouldReturnRecruiter_WhenExists()
    {
        var recruiter = new Recruiter { UserId = 1, FullName = "Recruiter", CompanyName = "TestCo" };
        await _repository.CreateRecruiterAsync(recruiter);
        var result = await _repository.GetRecruiterByUserIdAsync(1);
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetRecruiterByUserIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetRecruiterByUserIdAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateCandidateAsync_ShouldUpdateCandidate()
    {
        var candidate = new Candidate { UserId = 1, FullName = "Old", Email = "test@test.com", Skills = new List<string>() };
        await _repository.CreateCandidateAsync(candidate);
        candidate.FullName = "Updated";
        var result = await _repository.UpdateCandidateAsync(candidate);
        result.FullName.Should().Be("Updated");
    }

    [TestMethod]
    public async Task DeleteRecruiterAsync_ShouldReturnTrue_WhenExists()
    {
        var recruiter = new Recruiter { UserId = 1, FullName = "Rec", CompanyName = "Co" };
        await _repository.CreateRecruiterAsync(recruiter);
        var result = await _repository.DeleteRecruiterAsync(1);
        result.Should().BeTrue();
    }
}
