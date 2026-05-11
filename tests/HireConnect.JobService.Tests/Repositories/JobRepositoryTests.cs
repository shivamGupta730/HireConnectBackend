using FluentAssertions;
using HireConnect.JobService.Data;
using HireConnect.JobService.Models;
using HireConnect.JobService.Repositories;
using HireConnect.JobService.Tests.Helpers;
using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.JobService.Tests.Repositories;

[TestClass]
public class JobRepositoryTests
{
    private JobDbContext _context = null!;
    private JobRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<JobDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new TestJobDbContext(options);
        _repository = new JobRepository(_context);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    private static Job CreateTestJob(int postedBy = 1, string title = "Test Job",
        JobStatus status = JobStatus.Active)
    {
        return new Job
        {
            Title = title,
            Category = "IT",
            Type = "FullTime",
            Location = "Remote",
            IsRemote = true,
            SalaryMin = 50000,
            SalaryMax = 100000,
            Currency = "USD",
            Skills = new[] { "C#", ".NET" },
            ExperienceRequired = 3,
            Description = "Test description",
            PostedBy = postedBy,
            Status = status,
            PostedAt = DateTime.UtcNow
        };
    }

    [TestMethod]
    public async Task CreateAsync_ShouldCreateJob_WhenValid()
    {
        var job = CreateTestJob();
        var result = await _repository.CreateAsync(job);
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Job");
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnJob_WhenExists()
    {
        var job = CreateTestJob();
        await _repository.CreateAsync(job);
        var result = await _repository.GetByIdAsync(job.Id);
        result.Should().NotBeNull();
        result!.Title.Should().Be("Test Job");
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetOpenJobsAsync_ShouldReturnOnlyActiveJobs()
    {
        await _repository.CreateAsync(CreateTestJob(title: "Active", status: JobStatus.Active));
        await _repository.CreateAsync(CreateTestJob(title: "Closed", status: JobStatus.Closed));

        var result = await _repository.GetOpenJobsAsync(new BaseRequest { Page = 1, PageSize = 10 });
        result.Data.Should().HaveCount(1);
        result.Data[0].Title.Should().Be("Active");
    }

    [TestMethod]
    public async Task GetOpenJobsAsync_ShouldPaginateCorrectly()
    {
        for (int i = 0; i < 15; i++)
            await _repository.CreateAsync(CreateTestJob(title: $"Job {i}"));

        var page1 = await _repository.GetOpenJobsAsync(new BaseRequest { Page = 1, PageSize = 10 });
        var page2 = await _repository.GetOpenJobsAsync(new BaseRequest { Page = 2, PageSize = 10 });

        page1.Data.Should().HaveCount(10);
        page2.Data.Should().HaveCount(5);
        page1.TotalCount.Should().Be(15);
    }

    [TestMethod]
    public async Task GetJobsByRecruiterAsync_ShouldReturnOnlyRecruiterJobs()
    {
        await _repository.CreateAsync(CreateTestJob(postedBy: 1, title: "Recruiter1 Job"));
        await _repository.CreateAsync(CreateTestJob(postedBy: 2, title: "Recruiter2 Job"));

        var result = await _repository.GetJobsByRecruiterAsync(1, new BaseRequest { Page = 1, PageSize = 10 });
        result.Data.Should().HaveCount(1);
        result.Data[0].PostedBy.Should().Be(1);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateJob()
    {
        var job = CreateTestJob();
        await _repository.CreateAsync(job);
        job.Title = "Updated Title";

        var result = await _repository.UpdateAsync(job);
        result.Title.Should().Be("Updated Title");
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
    {
        var job = CreateTestJob();
        await _repository.CreateAsync(job);
        var result = await _repository.DeleteAsync(job.Id);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.DeleteAsync(999);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task UpdateStatusAsync_ShouldUpdateStatus()
    {
        var job = CreateTestJob(status: JobStatus.Active);
        await _repository.CreateAsync(job);

        var result = await _repository.UpdateStatusAsync(job.Id, JobStatus.Closed);
        result.Should().BeTrue();

        var updated = await _repository.GetByIdAsync(job.Id);
        updated!.Status.Should().Be(JobStatus.Closed);
    }

    [TestMethod]
    public async Task UpdateViewCountAsync_ShouldIncrementViewCount()
    {
        var job = CreateTestJob();
        await _repository.CreateAsync(job);

        await _repository.UpdateViewCountAsync(job.Id);
        var updated = await _repository.GetByIdAsync(job.Id);
        updated!.ViewCount.Should().Be(1);
    }
}
