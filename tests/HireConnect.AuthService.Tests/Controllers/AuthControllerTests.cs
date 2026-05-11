using FluentAssertions;
using HireConnect.AuthService.Controllers;
using HireConnect.AuthService.Dtos;
using HireConnect.AuthService.Services;
using HireConnect.AuthService.Tests.Helpers;
using HireConnect.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HireConnect.AuthService.Tests.Controllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private Mock<IGitHubOAuthService> _gitHubOAuthServiceMock = null!;
    private Mock<ILogger<AuthController>> _loggerMock = null!;
    private AuthController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _gitHubOAuthServiceMock = new Mock<IGitHubOAuthService>();
        _loggerMock = new Mock<ILogger<AuthController>>();

        _controller = new AuthController(
            _authServiceMock.Object,
            _gitHubOAuthServiceMock.Object,
            _loggerMock.Object
        );
    }

    [TestMethod]
    public async Task Register_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var request = TestDataSeeder.CreateRegisterRequest();
        var loginResponse = new LoginResponse
        {
            Token = "test-token",
            UserId = 1,
            Role = UserRole.Candidate,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(loginResponse);

        // Act
        var actionResult = await _controller.Register(request);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var response = okResult.Value.Should().BeOfType<ApiResponse<LoginResponse>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Token.Should().Be("test-token");
    }

    [TestMethod]
    public async Task Register_ShouldReturnBadRequest_WhenEmailExists()
    {
        // Arrange
        var request = TestDataSeeder.CreateRegisterRequest();
        _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ThrowsAsync(new InvalidOperationException("Email already exists"));

        // Act
        var actionResult = await _controller.Register(request);

        // Assert
        var badResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badResult.StatusCode.Should().Be(400);
        var response = badResult.Value.Should().BeOfType<ApiResponse<LoginResponse>>().Subject;
        response.Success.Should().BeFalse();
    }

    [TestMethod]
    public async Task Login_ShouldReturnOk_WhenValidCredentials()
    {
        // Arrange
        var request = TestDataSeeder.CreateLoginRequest();
        var loginResponse = new LoginResponse
        {
            Token = "test-token",
            UserId = 1,
            Role = UserRole.Candidate,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(loginResponse);

        // Act
        var actionResult = await _controller.Login(request);

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var response = okResult.Value.Should().BeOfType<ApiResponse<LoginResponse>>().Subject;
        response.Success.Should().BeTrue();
    }

    [TestMethod]
    public async Task Login_ShouldReturnBadRequest_WhenInvalidCredentials()
    {
        // Arrange
        var request = TestDataSeeder.CreateLoginRequest();
        _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
            .ThrowsAsync(new InvalidOperationException("Invalid credentials"));

        // Act
        var actionResult = await _controller.Login(request);

        // Assert
        var badResult = actionResult.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badResult.StatusCode.Should().Be(400);
        var response = badResult.Value.Should().BeOfType<ApiResponse<LoginResponse>>().Subject;
        response.Success.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        var userDto = new UserResponseDto
        {
            Id = 1,
            Email = "test@test.com",
            Role = UserRole.Candidate,
            CreatedAt = DateTime.UtcNow
        };

        _authServiceMock.Setup(s => s.GetUserByIdAsync(1))
            .ReturnsAsync(userDto);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = JwtTestHelper.CreateClaimsPrincipal(1, "test@test.com", "Candidate")
            }
        };

        // Act
        var actionResult = await _controller.GetProfile();

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var response = okResult.Value.Should().BeOfType<ApiResponse<UserResponseDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data!.Email.Should().Be("test@test.com");
    }

    [TestMethod]
    public async Task GetProfile_ShouldReturnNotFound_WhenUserNotFound()
    {
        // Arrange
        _authServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((UserResponseDto?)null);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = JwtTestHelper.CreateClaimsPrincipal(999, "none@test.com", "Candidate")
            }
        };

        // Act
        var actionResult = await _controller.GetProfile();

        // Assert
        var notFound = actionResult.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.StatusCode.Should().Be(404);
    }

    [TestMethod]
    public void Validate_ShouldReturnOk_WhenAuthenticated()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = JwtTestHelper.CreateClaimsPrincipal(1, "test@test.com", "Candidate")
            }
        };

        // Act
        var result = _controller.Validate();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }
}
