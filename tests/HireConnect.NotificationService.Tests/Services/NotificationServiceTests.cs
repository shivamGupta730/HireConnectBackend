using FluentAssertions;
using HireConnect.NotificationService.Models;
using HireConnect.NotificationService.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HireConnect.NotificationService.Tests.Services;

[TestClass]
public class NotificationServiceTests
{
    private Mock<INotificationRepository> _repoMock = null!;
    private Mock<ILogger<HireConnect.NotificationService.Services.NotificationService>> _loggerMock = null!;
    private HireConnect.NotificationService.Services.NotificationService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repoMock = new Mock<INotificationRepository>();
        _loggerMock = new Mock<ILogger<HireConnect.NotificationService.Services.NotificationService>>();
        _service = new HireConnect.NotificationService.Services.NotificationService(_repoMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task SendNotification_ShouldReturnCreatedNotification()
    {
        var notification = new Notification { UserId = 1, Type = "Test", Message = "Hello" };
        _repoMock.Setup(r => r.CreateAsync(It.IsAny<Notification>()))
            .ReturnsAsync((Notification n) => { n.NotificationId = 1; return n; });

        var result = await _service.SendNotification(notification);
        result.NotificationId.Should().Be(1);
    }

    [TestMethod]
    public async Task GetByUser_ShouldReturnNotifications()
    {
        var notifications = new List<Notification>
        {
            new() { NotificationId = 1, UserId = 1, Type = "A", Message = "M1" },
            new() { NotificationId = 2, UserId = 1, Type = "B", Message = "M2" }
        };
        _repoMock.Setup(r => r.FindByUserId(1)).ReturnsAsync(notifications);

        var result = await _service.GetByUser(1);
        result.Should().HaveCount(2);
    }

    [TestMethod]
    public async Task GetUnreadCount_ShouldReturnCount()
    {
        _repoMock.Setup(r => r.CountByUserIdAndIsRead(1, false)).ReturnsAsync(5);
        var result = await _service.GetUnreadCount(1);
        result.Should().Be(5);
    }

    [TestMethod]
    public async Task DeleteNotification_ShouldReturnTrue_WhenExists()
    {
        _repoMock.Setup(r => r.DeleteByNotificationId(1)).ReturnsAsync(true);
        var result = await _service.DeleteNotification(1);
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteNotification_ShouldReturnFalse_WhenNotExists()
    {
        _repoMock.Setup(r => r.DeleteByNotificationId(999)).ReturnsAsync(false);
        var result = await _service.DeleteNotification(999);
        result.Should().BeFalse();
    }

    [TestMethod]
    public async Task MarkAllRead_ShouldMarkAllAsRead()
    {
        var notifications = new List<Notification>
        {
            new() { NotificationId = 1, UserId = 1, IsRead = false, Type = "A", Message = "M" },
            new() { NotificationId = 2, UserId = 1, IsRead = false, Type = "B", Message = "M" }
        };
        _repoMock.Setup(r => r.FindByUserIdAndIsRead(1, false)).ReturnsAsync(notifications);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Notification>()))
            .ReturnsAsync((Notification n) => n);

        var result = await _service.MarkAllRead(1);
        result.Should().BeTrue();
        _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Notification>()), Times.Exactly(2));
    }
}
