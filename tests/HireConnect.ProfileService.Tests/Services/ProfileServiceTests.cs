using FluentAssertions;
using HireConnect.ProfileService.Repositories;
using HireConnect.ProfileService.Services;
using HireConnect.Shared.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HireConnect.ProfileService.Tests.Services;

[TestClass]
public class ProfileServiceTests
{
    private Mock<IProfileRepository> _repoMock = null!;
    private HireConnect.ProfileService.Services.ProfileService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repoMock = new Mock<IProfileRepository>();
        _service = new HireConnect.ProfileService.Services.ProfileService(_repoMock.Object);
    }

    [TestMethod]
    public async Task GetMyProfileAsync_ShouldReturnCandidate_WhenExists()
    {
        var candidate = new Candidate { Id = 1, UserId = 1, FullName = "Test", Email = "t@t.com", Skills = new List<string>() };
        _repoMock.Setup(r => r.GetCandidateByUserIdAsync(1)).ReturnsAsync(candidate);
        var result = await _service.GetMyProfileAsync(1);
        result.Should().NotBeNull();
        result!.FullName.Should().Be("Test");
    }

    [TestMethod]
    public async Task GetMyProfileAsync_ShouldReturnNull_WhenNotExists()
    {
        _repoMock.Setup(r => r.GetCandidateByUserIdAsync(999)).ReturnsAsync((Candidate?)null);
        var result = await _service.GetMyProfileAsync(999);
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task CreateCandidateAsync_ShouldCallRepository()
    {
        var candidate = new Candidate { UserId = 1, FullName = "New", Email = "n@t.com", Skills = new List<string>() };
        _repoMock.Setup(r => r.CreateCandidateAsync(It.IsAny<Candidate>()))
            .ReturnsAsync((Candidate c) => { c.Id = 1; return c; });
        var result = await _service.CreateCandidateAsync(candidate);
        result.Id.Should().Be(1);
        _repoMock.Verify(r => r.CreateCandidateAsync(It.IsAny<Candidate>()), Times.Once);
    }

    [TestMethod]
    public async Task GetMyRecruiterProfileAsync_ShouldReturnRecruiter()
    {
        var recruiter = new Recruiter { Id = 1, UserId = 1, FullName = "Rec", CompanyName = "Co" };
        _repoMock.Setup(r => r.GetRecruiterByUserIdAsync(1)).ReturnsAsync(recruiter);
        var result = await _service.GetMyRecruiterProfileAsync(1);
        result.Should().NotBeNull();
    }

    [TestMethod]
    public async Task CreateRecruiterAsync_ShouldCallRepository()
    {
        var recruiter = new Recruiter { UserId = 1, FullName = "Rec", CompanyName = "Co" };
        _repoMock.Setup(r => r.CreateRecruiterAsync(It.IsAny<Recruiter>()))
            .ReturnsAsync((Recruiter r) => { r.Id = 1; return r; });
        var result = await _service.CreateRecruiterAsync(recruiter);
        result.Id.Should().Be(1);
    }

    [TestMethod]
    public async Task DeleteCandidateAsync_ShouldReturnTrue_WhenDeleted()
    {
        _repoMock.Setup(r => r.DeleteCandidateAsync(1)).ReturnsAsync(true);
        var result = await _service.DeleteCandidateAsync(1);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteRecruiterAsync_ShouldReturnFalse_WhenNotExists()
    {
        _repoMock.Setup(r => r.DeleteRecruiterAsync(999)).ReturnsAsync(false);
        var result = await _service.DeleteRecruiterAsync(999);
        result.Should().BeFalse();
    }
}
