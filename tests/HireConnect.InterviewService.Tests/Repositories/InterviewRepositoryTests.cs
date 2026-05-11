using FluentAssertions;
using HireConnect.InterviewService.Data;
using HireConnect.InterviewService.DTOs;
using HireConnect.InterviewService.Models;
using HireConnect.InterviewService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.InterviewService.Tests.Repositories;

[TestClass]
public class InterviewRepositoryTests
{
    private InterviewDbContext _context = null!;
    private InterviewRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new InterviewDbContext(options);
        _repository = new InterviewRepository(_context);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    [TestMethod]
    public async Task CreateAsync_ShouldCreateInterview()
    {
        var interview = new Interview
        {
            ApplicationId = 1, JobId = 1, CandidateId = 1,
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            Status = InterviewStatus.Scheduled, CreatedAt = DateTime.UtcNow
        };
        var result = await _repository.CreateAsync(interview);
        result.Id.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        var interview = new Interview
        {
            ApplicationId = 1, JobId = 1, CandidateId = 1,
            ScheduledAt = DateTime.UtcNow.AddDays(1),
            Status = InterviewStatus.Scheduled, CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(interview);
        var result = await _repository.UpdateStatusAsync(interview.Id, InterviewStatus.Completed);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.DeleteAsync(999);
        result.Should().BeFalse();
    }
}
