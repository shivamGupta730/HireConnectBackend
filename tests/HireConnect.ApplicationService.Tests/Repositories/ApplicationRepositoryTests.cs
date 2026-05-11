using FluentAssertions;
using HireConnect.ApplicationService.Data;
using HireConnect.ApplicationService.DTOs;
using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.ApplicationService.Tests.Repositories;

[TestClass]
public class ApplicationRepositoryTests
{
    private ApplicationDbContext _context = null!;
    private ApplicationRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _repository = new ApplicationRepository(_context);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    private static Application CreateTestApplication(int jobId = 1, int candidateId = 1)
    {
        return new Application
        {
            JobId = jobId,
            CandidateId = candidateId,
            Status = ApplicationStatus.Applied,
            CoverLetter = "Test cover letter",
            AppliedAt = DateTime.UtcNow
        };
    }

    [TestMethod]
    public async Task CreateAsync_ShouldCreateApplication()
    {
        var app = CreateTestApplication();
        var result = await _repository.CreateAsync(app);
        result.Id.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnApplication_WhenExists()
    {
        var app = CreateTestApplication();
        await _repository.CreateAsync(app);
        var result = await _repository.GetByIdAsync(app.Id);
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetByJobAndCandidateAsync_ShouldReturnApplication()
    {
        await _repository.CreateAsync(CreateTestApplication(jobId: 1, candidateId: 1));
        var result = await _repository.GetByJobAndCandidateAsync(1, 1);
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task GetByJobAndCandidateAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByJobAndCandidateAsync(999, 999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetApplicationsByCandidateAsync_ShouldReturnPaged()
    {
        await _repository.CreateAsync(CreateTestApplication(jobId: 1, candidateId: 1));
        await _repository.CreateAsync(CreateTestApplication(jobId: 2, candidateId: 1));

        var result = await _repository.GetApplicationsByCandidateAsync(1, new BaseRequest { Page = 1, PageSize = 10 });
        result.Data.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [TestMethod]
    public async Task GetApplicationsByJobAsync_ShouldReturnPaged()
    {
        await _repository.CreateAsync(CreateTestApplication(jobId: 1, candidateId: 1));
        await _repository.CreateAsync(CreateTestApplication(jobId: 1, candidateId: 2));

        var result = await _repository.GetApplicationsByJobAsync(1, new BaseRequest { Page = 1, PageSize = 10 });
        result.Data.Should().HaveCount(2);
    }

    [TestMethod]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        var app = CreateTestApplication();
        await _repository.CreateAsync(app);
        var result = await _repository.UpdateStatusAsync(app.Id, ApplicationStatus.Shortlisted, "Good candidate");
        result.Should().BeTrue();

        var updated = await _repository.GetByIdAsync(app.Id);
        updated!.Status.Should().Be(ApplicationStatus.Shortlisted);
        updated.Notes.Should().Be("Good candidate");
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
    {
        var app = CreateTestApplication();
        await _repository.CreateAsync(app);
        var result = await _repository.DeleteAsync(app.Id);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.DeleteAsync(999);
        result.Should().BeFalse();
    }
}
