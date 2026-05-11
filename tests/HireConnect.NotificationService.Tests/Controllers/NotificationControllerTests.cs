using FluentAssertions;
using HireConnect.NotificationService.Controllers;
using HireConnect.NotificationService.DTOs;
using HireConnect.NotificationService.Models;
using HireConnect.NotificationService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HireConnect.NotificationService.Tests.Controllers;

[TestClass]
public class NotificationControllerTests
{
    private Mock<INotificationService> _serviceMock = null!;
    private Mock<ILogger<NotificationController>> _loggerMock = null!;
    private NotificationController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<NotificationController>>();
        _controller = new NotificationController(_serviceMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task CreateNotification_ShouldReturnCreated_WhenValid()
    {
        var dto = new CreateNotificationDto { UserId = 1, Type = "Test", Message = "Hello" };
        _serviceMock.Setup(s => s.SendNotification(It.IsAny<Notification>()))
            .ReturnsAsync(new Notification { NotificationId = 1, UserId = 1, Type = "Test", Message = "Hello" });

        var result = await _controller.CreateNotification(dto);
        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.StatusCode.Should().Be(201);
    }

    [TestMethod]
    public async Task GetNotifications_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetByUser(1))
            .ReturnsAsync(new List<Notification>());

        var result = await _controller.GetNotifications(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task GetUnreadCount_ShouldReturnOk()
    {
        _serviceMock.Setup(s => s.GetUnreadCount(1)).ReturnsAsync(3);
        var result = await _controller.GetUnreadCount(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task MarkAsRead_ShouldReturnOk_WhenFound()
    {
        _serviceMock.Setup(s => s.MarkAsRead(1)).ReturnsAsync(true);
        var result = await _controller.MarkAsRead(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task MarkAsRead_ShouldReturnNotFound_WhenNotFound()
    {
        _serviceMock.Setup(s => s.MarkAsRead(999)).ReturnsAsync(false);
        var result = await _controller.MarkAsRead(999);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [TestMethod]
    public async Task DeleteNotification_ShouldReturnOk_WhenDeleted()
    {
        _serviceMock.Setup(s => s.DeleteNotification(1)).ReturnsAsync(true);
        var result = await _controller.DeleteNotification(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [TestMethod]
    public async Task DeleteNotification_ShouldReturnNotFound_WhenNotExists()
    {
        _serviceMock.Setup(s => s.DeleteNotification(999)).ReturnsAsync(false);
        var result = await _controller.DeleteNotification(999);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [TestMethod]
    public async Task MarkAllAsRead_ShouldReturnOk_WhenSuccess()
    {
        _serviceMock.Setup(s => s.MarkAllRead(1)).ReturnsAsync(true);
        var result = await _controller.MarkAllAsRead(1);
        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
