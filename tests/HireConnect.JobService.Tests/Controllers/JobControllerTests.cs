using FluentAssertions;
using HireConnect.JobService.Controllers;
using HireConnect.JobService.DTOs;
using HireConnect.JobService.Services;
using HireConnect.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using JobStatus = HireConnect.JobService.Models.JobStatus;

namespace HireConnect.JobService.Tests.Controllers;

[TestClass]
public class JobControllerTests
{
    private Mock<IJobService> _jobServiceMock = null!;
    private Mock<ILogger<JobController>> _loggerMock = null!;
    private JobController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _jobServiceMock = new Mock<IJobService>();
        _loggerMock = new Mock<ILogger<JobController>>();
        _controller = new JobController(_jobServiceMock.Object, _loggerMock.Object);
    }

    private void SetupRecruiterUser(int userId = 1)
    {
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Recruiter")
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    [TestMethod]
    public async Task CreateJob_ShouldReturnCreatedAtAction_WhenValid()
    {
        SetupRecruiterUser();
        var dto = new CreateJobDto
        {
            Title = "Job",
            Category = "IT",
            Type = "FullTime",
            Location = "Remote",
            SalaryMin = 50000,
            Description = "Test",
            Skills = new List<string>()
        };
        var response = new JobResponseDto { Id = 1, Title = "Job" };

        _jobServiceMock.Setup(s => s.CreateJobAsync(1, It.IsAny<CreateJobDto>()))
            .ReturnsAsync(response);

        var result = await _controller.CreateJob(dto);
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
    }

    [TestMethod]
    public async Task GetJobs_ShouldReturnOk()
    {
        var pagedResponse = new PagedResponse<JobResponseDto>
        {
            Data = new List<JobResponseDto>(),
            Page = 1,
            PageSize = 10,
            TotalCount = 0
        };
        _jobServiceMock.Setup(s => s.GetOpenJobsAsync(It.IsAny<BaseRequest>()))
            .ReturnsAsync(pagedResponse);

        var result = await _controller.GetJobs(new BaseRequest());
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task GetJob_ShouldReturnNotFound_WhenJobDoesNotExist()
    {
        _jobServiceMock.Setup(s => s.GetJobByIdAsync(999)).ReturnsAsync((JobResponseDto?)null);
        var result = await _controller.GetJob(999);
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task GetJob_ShouldReturnOk_WhenJobExists()
    {
        var job = new JobResponseDto { Id = 1, Title = "Test" };
        _jobServiceMock.Setup(s => s.GetJobByIdAsync(1)).ReturnsAsync(job);
        var result = await _controller.GetJob(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task DeleteJob_ShouldReturnNoContent_WhenDeleted()
    {
        SetupRecruiterUser();
        _jobServiceMock.Setup(s => s.DeleteJobAsync(1, 1)).ReturnsAsync(true);
        var result = await _controller.DeleteJob(1);
        result.Should().BeOfType<NoContentResult>();
    }

    [TestMethod]
    public async Task DeleteJob_ShouldReturnNotFound_WhenNotExists()
    {
        SetupRecruiterUser();
        _jobServiceMock.Setup(s => s.DeleteJobAsync(999, 1)).ReturnsAsync(false);
        var result = await _controller.DeleteJob(999);
        result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task GetJobsByRecruiter_ShouldReturnForbid_WhenNotOwner()
    {
        SetupRecruiterUser(userId: 1);
        var result = await _controller.GetJobsByRecruiter(2, new BaseRequest());
        result.Result.Should().BeOfType<ForbidResult>();
    }

    [TestMethod]
    public async Task UpdateJobStatus_ShouldReturnNoContent_WhenUpdated()
    {
        SetupRecruiterUser();
        _jobServiceMock.Setup(s => s.UpdateJobStatusAsync(1, 1, JobStatus.Closed)).ReturnsAsync(true);
        var dto = new UpdateJobStatusDto { Status = JobStatus.Closed };
        var result = await _controller.UpdateJobStatus(1, dto);
        result.Should().BeOfType<NoContentResult>();
    }
}
