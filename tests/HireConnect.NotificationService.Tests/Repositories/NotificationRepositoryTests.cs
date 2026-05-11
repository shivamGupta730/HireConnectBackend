using FluentAssertions;
using HireConnect.NotificationService.Data;
using HireConnect.NotificationService.Models;
using HireConnect.NotificationService.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.NotificationService.Tests.Repositories;

[TestClass]
public class NotificationRepositoryTests
{
    private NotificationDbContext _context = null!;
    private NotificationRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new NotificationDbContext(options);
        _repository = new NotificationRepository(_context);
    }

    [TestCleanup]
    public void Cleanup() => _context.Dispose();

    private static Notification CreateTestNotification(int userId = 1, bool isRead = false)
    {
        return new Notification
        {
            UserId = userId,
            Type = "Application",
            Message = "Test notification",
            IsRead = isRead,
            CreatedAt = DateTime.UtcNow
        };
    }

    [TestMethod]
    public async Task CreateAsync_ShouldCreateNotification()
    {
        var n = CreateTestNotification();
        var result = await _repository.CreateAsync(n);
        result.NotificationId.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public async Task FindByUserId_ShouldReturnUserNotifications()
    {
        await _repository.CreateAsync(CreateTestNotification(userId: 1));
        await _repository.CreateAsync(CreateTestNotification(userId: 1));
        await _repository.CreateAsync(CreateTestNotification(userId: 2));

        var result = await _repository.FindByUserId(1);
        result.Should().HaveCount(2);
    }

    [TestMethod]
    public async Task FindByUserIdAndIsRead_ShouldFilterCorrectly()
    {
        await _repository.CreateAsync(CreateTestNotification(userId: 1, isRead: false));
        await _repository.CreateAsync(CreateTestNotification(userId: 1, isRead: true));

        var unread = await _repository.FindByUserIdAndIsRead(1, false);
        unread.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task CountByUserIdAndIsRead_ShouldReturnCorrectCount()
    {
        await _repository.CreateAsync(CreateTestNotification(userId: 1, isRead: false));
        await _repository.CreateAsync(CreateTestNotification(userId: 1, isRead: false));
        await _repository.CreateAsync(CreateTestNotification(userId: 1, isRead: true));

        var count = await _repository.CountByUserIdAndIsRead(1, false);
        count.Should().Be(2);
    }

    [TestMethod]
    public async Task DeleteByNotificationId_ShouldReturnTrue_WhenExists()
    {
        var n = CreateTestNotification();
        await _repository.CreateAsync(n);
        var result = await _repository.DeleteByNotificationId(n.NotificationId);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteByNotificationId_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _repository.DeleteByNotificationId(999);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateNotification()
    {
        var n = CreateTestNotification(isRead: false);
        await _repository.CreateAsync(n);
        n.IsRead = true;
        var result = await _repository.UpdateAsync(n);
        result.IsRead.Should().BeTrue();
    }
}
