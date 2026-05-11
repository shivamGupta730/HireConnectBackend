using FluentAssertions;
using HireConnect.ApplicationService.DTOs;
using HireConnect.ApplicationService.Models;
using HireConnect.ApplicationService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace HireConnect.ApplicationService.Tests.Services;

[TestClass]
public class ApplicationServiceTests
{
    private Mock<IApplicationRepository> _repoMock = null!;
    private Mock<ILogger<HireConnect.ApplicationService.Services.ApplicationService>> _loggerMock = null!;
    private IConfiguration _config = null!;
    private HttpClient _httpClient = null!;
    private Mock<HttpMessageHandler> _httpHandlerMock = null!;
    private HireConnect.ApplicationService.Services.ApplicationService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repoMock = new Mock<IApplicationRepository>();
        _loggerMock = new Mock<ILogger<HireConnect.ApplicationService.Services.ApplicationService>>();
        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ServiceUrls:JobService", "http://localhost:5003" },
                { "ServiceUrls:NotificationService", "http://localhost:5006" }
            }).Build();

        _httpHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpHandlerMock.Object);
        _service = new HireConnect.ApplicationService.Services.ApplicationService(
            _repoMock.Object, _httpClient, _config, _loggerMock.Object);
    }

    private void SetupJobServiceResponse(int jobId, int postedBy, int status = 1)
    {
        var jobResponse = JsonSerializer.Serialize(new { id = jobId, postedBy = postedBy, status = status });
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains($"/api/job/{jobId}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jobResponse)
            });
    }

    private void SetupNotificationResponse()
    {
        _httpHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.ToString().Contains("/api/Notification")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));
    }

    [TestMethod]
    public async Task CreateApplicationAsync_ShouldCreate_WhenValid()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByJobAndCandidateAsync(1, 1)).ReturnsAsync((Application?)null);
        SetupJobServiceResponse(1, postedBy: 10, status: 1);
        SetupNotificationResponse();
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Application>()))
            .ReturnsAsync((Application a) => { a.Id = 1; return a; });

        var dto = new CreateApplicationDto { JobId = 1 };

        // Act
        var result = await _service.CreateApplicationAsync(1, dto);

        // Assert
        result.Should().NotBeNull();
        result.JobId.Should().Be(1);
        result.CandidateId.Should().Be(1);
    }

    [TestMethod]
    public async Task CreateApplicationAsync_ShouldThrow_WhenAlreadyApplied()
    {
        _repoMock.Setup(r => r.GetByJobAndCandidateAsync(1, 1))
            .ReturnsAsync(new Application { Id = 1 });

        var dto = new CreateApplicationDto { JobId = 1 };
        Func<Task> act = async () => await _service.CreateApplicationAsync(1, dto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already applied*");
    }

    [TestMethod]
    public async Task DeleteApplicationAsync_ShouldThrow_WhenNotOwner()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Application { Id = 1, CandidateId = 2 });

        Func<Task> act = async () => await _service.DeleteApplicationAsync(1, 999);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [TestMethod]
    public async Task UpdateApplicationStatusAsync_ShouldThrow_WhenInvalidTransition()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Application { Id = 1, JobId = 1, Status = ApplicationStatus.Applied });
        SetupJobServiceResponse(1, postedBy: 1);

        var dto = new UpdateApplicationStatusDto { Status = ApplicationStatus.Offered };
        Func<Task> act = async () => await _service.UpdateApplicationStatusAsync(1, 1, dto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Invalid status transition*");
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Application?)null);
        var result = await _service.GetByIdAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnDto_WhenExists()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Application { Id = 1, JobId = 1, CandidateId = 1, Status = ApplicationStatus.Applied, AppliedAt = DateTime.UtcNow });
        var result = await _service.GetByIdAsync(1);
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }
}
