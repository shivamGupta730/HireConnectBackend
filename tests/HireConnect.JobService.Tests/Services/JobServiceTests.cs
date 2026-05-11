using FluentAssertions;
using HireConnect.JobService.DTOs;
using HireConnect.JobService.Models;
using HireConnect.JobService.Repositories;
using HireConnect.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobStatus = HireConnect.JobService.Models.JobStatus;

namespace HireConnect.JobService.Tests.Services;

[TestClass]
public class JobServiceTests
{
    private Mock<IJobRepository> _jobRepositoryMock = null!;
    private HireConnect.JobService.Services.JobService _jobService = null!;

    [TestInitialize]
    public void Setup()
    {
        _jobRepositoryMock = new Mock<IJobRepository>();
        _jobService = new HireConnect.JobService.Services.JobService(_jobRepositoryMock.Object);
    }

    private static Job CreateTestJob(int id = 1, int postedBy = 1)
    {
        return new Job
        {
            Id = id,
            Title = "Test Job",
            Category = "IT",
            Type = "FullTime",
            Location = "Remote",
            IsRemote = true,
            SalaryMin = 50000,
            SalaryMax = 100000,
            Currency = "USD",
            Skills = new[] { "C#" },
            ExperienceRequired = 3,
            Description = "Test",
            PostedBy = postedBy,
            Status = JobStatus.Active,
            PostedAt = DateTime.UtcNow
        };
    }

    [TestMethod]
    public async Task CreateJobAsync_ShouldReturnJobResponse_WhenValid()
    {
        // Arrange
        var dto = new CreateJobDto
        {
            Title = "New Job",
            Category = "IT",
            Type = "FullTime",
            Location = "Remote",
            SalaryMin = 50000,
            Description = "Test",
            Skills = new List<string> { "C#" }
        };

        _jobRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Job>()))
            .ReturnsAsync((Job j) => { j.Id = 1; return j; });

        // Act
        var result = await _jobService.CreateJobAsync(1, dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Job");
        result.PostedBy.Should().Be(1);
    }

    [TestMethod]
    public async Task CreateJobAsync_ShouldThrow_WhenMinSalaryGreaterThanMax()
    {
        var dto = new CreateJobDto
        {
            Title = "Job",
            Category = "IT",
            Type = "FullTime",
            Location = "Remote",
            SalaryMin = 100000,
            SalaryMax = 50000,
            Description = "Test",
            Skills = new List<string>()
        };

        Func<Task> act = async () => await _jobService.CreateJobAsync(1, dto);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*salary*");
    }

    [TestMethod]
    public async Task GetJobByIdAsync_ShouldReturnJob_WhenExists()
    {
        var job = CreateTestJob();
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
        _jobRepositoryMock.Setup(r => r.UpdateViewCountAsync(1)).ReturnsAsync(true);

        var result = await _jobService.GetJobByIdAsync(1);
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [TestMethod]
    public async Task GetJobByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);
        var result = await _jobService.GetJobByIdAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateJobAsync_ShouldThrow_WhenNotOwner()
    {
        var job = CreateTestJob(postedBy: 1);
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);

        var dto = new UpdateJobDto { Title = "Updated" };
        Func<Task> act = async () => await _jobService.UpdateJobAsync(1, 999, dto);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [TestMethod]
    public async Task UpdateJobAsync_ShouldReturnNull_WhenJobNotFound()
    {
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);
        var result = await _jobService.UpdateJobAsync(999, 1, new UpdateJobDto());
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteJobAsync_ShouldThrow_WhenNotOwner()
    {
        var job = CreateTestJob(postedBy: 1);
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);

        Func<Task> act = async () => await _jobService.DeleteJobAsync(1, 999);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [TestMethod]
    public async Task DeleteJobAsync_ShouldReturnFalse_WhenJobNotFound()
    {
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);
        var result = await _jobService.DeleteJobAsync(999, 1);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task UpdateJobStatusAsync_ShouldThrow_WhenNotOwner()
    {
        var job = CreateTestJob(postedBy: 1);
        _jobRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);

        Func<Task> act = async () => await _jobService.UpdateJobStatusAsync(1, 999, JobStatus.Closed);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [TestMethod]
    public async Task GetOpenJobsAsync_ShouldReturnPagedResponse()
    {
        var pagedResponse = new PagedResponse<Job>
        {
            Data = new List<Job> { CreateTestJob() },
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };

        _jobRepositoryMock.Setup(r => r.GetOpenJobsAsync(It.IsAny<BaseRequest>()))
            .ReturnsAsync(pagedResponse);

        var result = await _jobService.GetOpenJobsAsync(new BaseRequest { Page = 1, PageSize = 10 });
        result.Data.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
    }
}
