using FluentAssertions;
using HireConnect.AuthService.Data;
using HireConnect.AuthService.Repositories;
using HireConnect.AuthService.Tests.Helpers;
using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.AuthService.Tests.Repositories;

[TestClass]
public class UserRepositoryTests
{
    private AuthDbContext _context = null!;
    private UserRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextFactory.CreateInMemoryContext();
        _repository = new UserRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
    }

    [TestMethod]
    public async Task CreateAsync_ShouldCreateUser_WhenValidData()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 0, email: "create@test.com");

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Email.Should().Be("create@test.com");
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 0, email: "getbyid@test.com");
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("getbyid@test.com");
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 0, email: "email@test.com");
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByEmailAsync("email@test.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("email@test.com");
    }

    [TestMethod]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 0, email: "exists@test.com");
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.EmailExistsAsync("exists@test.com");

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _repository.EmailExistsAsync("notexists@test.com");

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateUser_WhenValid()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 0, email: "update@test.com");
        await _repository.CreateAsync(user);
        user.Email = "updated@test.com";

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.Email.Should().Be("updated@test.com");
        result.UpdatedAt.Should().NotBeNull();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var user = TestDataSeeder.CreateTestUser(id: 0, email: "delete@test.com");
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.DeleteAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _repository.GetByIdAsync(user.Id);
        deleted.Should().BeNull();
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        await _repository.CreateAsync(TestDataSeeder.CreateTestUser(id: 0, email: "user1@test.com"));
        await _repository.CreateAsync(TestDataSeeder.CreateTestUser(id: 0, email: "user2@test.com"));

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
