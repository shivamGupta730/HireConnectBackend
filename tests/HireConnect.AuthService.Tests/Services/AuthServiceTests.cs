using FluentAssertions;
using HireConnect.AuthService.Repositories;
using HireConnect.AuthService.Tests.Helpers;
using HireConnect.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HireConnect.AuthService.Tests.Services;

[TestClass]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private IConfiguration _configuration = null!;
    private HireConnect.AuthService.Services.AuthService _authService = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        var inMemorySettings = new Dictionary<string, string?>
        {
            { "JwtSettings:SecretKey", JwtTestHelper.TestSecretKey },
            { "JwtSettings:Issuer", JwtTestHelper.TestIssuer },
            { "JwtSettings:Audience", JwtTestHelper.TestAudience },
            { "JwtSettings:ExpirationMinutes", "60" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _authService = new HireConnect.AuthService.Services.AuthService(
            _userRepositoryMock.Object,
            _configuration
        );
    }

    [TestMethod]
    public async Task RegisterAsync_ShouldReturnLoginResponse_WhenValidRequest()
    {
        // Arrange
        var request = TestDataSeeder.CreateRegisterRequest();
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => { u.Id = 1; return u; });

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(1);
        result.Role.Should().Be(UserRole.Candidate);
    }

    [TestMethod]
    public async Task RegisterAsync_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = TestDataSeeder.CreateRegisterRequest();
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already*");
    }

    [TestMethod]
    public async Task LoginAsync_ShouldReturnLoginResponse_WhenValidCredentials()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Test123!");
        var user = TestDataSeeder.CreateTestUser(id: 1, email: "test@test.com", passwordHash: hashedPassword);

        _userRepositoryMock.Setup(r => r.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(user);

        var request = TestDataSeeder.CreateLoginRequest(email: "test@test.com", password: "Test123!");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(1);
    }

    [TestMethod]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var request = TestDataSeeder.CreateLoginRequest();

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Invalid*");
    }

    [TestMethod]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsInvalid()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = TestDataSeeder.CreateTestUser(id: 1, passwordHash: hashedPassword);

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var request = TestDataSeeder.CreateLoginRequest(password: "WrongPassword");

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Invalid*");
    }

    [TestMethod]
    public async Task GetUserByIdAsync_ShouldReturnDto_WhenUserExists()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 1, email: "dto@test.com", role: UserRole.Recruiter);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Email.Should().Be("dto@test.com");
        result.Role.Should().Be(UserRole.Recruiter);
    }

    [TestMethod]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.GetUserByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
