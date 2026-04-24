using HireConnect.NotificationService.Services;
using HireConnect.NotificationService.Models;
using HireConnect.NotificationService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HireConnect.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    // POST /api/notifications
    [HttpPost]
    public async Task<ActionResult<ApiResponse<NotificationResponseDto>>> CreateNotification([FromBody] CreateNotificationDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ApiResponse<NotificationResponseDto>
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            });
        }

        try
        {
            _logger.LogInformation("Creating notification for user {UserId} with type {Type}", request.UserId, request.Type);

            var notification = new Notification
            {
                UserId = request.UserId,
                Type = request.Type,
                Message = request.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var createdNotification = await _notificationService.SendNotification(notification);
            var responseDto = MapToNotificationResponseDto(createdNotification);

            _logger.LogInformation("Notification created successfully with ID {NotificationId}", createdNotification.NotificationId);

            return CreatedAtAction(nameof(GetNotifications), new { userId = notification.UserId }, new ApiResponse<NotificationResponseDto>
            {
                Success = true,
                Message = "Notification created successfully",
                Data = responseDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification for user {UserId}", request.UserId);
            return StatusCode(500, new ApiResponse<NotificationResponseDto>
            {
                Success = false,
                Message = "An error occurred while creating the notification",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // GET /api/notifications/{userId}
    [HttpGet("{userId}")]
    public async Task<ActionResult<ApiResponse<List<NotificationResponseDto>>>> GetNotifications(int userId)
    {
        try
        {
            _logger.LogInformation("Retrieving notifications for user {UserId}", userId);

            var notifications = await _notificationService.GetByUser(userId);
            var responseDtos = notifications.Select(MapToNotificationResponseDto).ToList();

            _logger.LogInformation("Retrieved {Count} notifications for user {UserId}", responseDtos.Count, userId);

            return Ok(new ApiResponse<List<NotificationResponseDto>>
            {
                Success = true,
                Message = "Notifications retrieved successfully",
                Data = responseDtos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve notifications for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<NotificationResponseDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving notifications",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // GET /api/notifications/{userId}/unread-count
    [HttpGet("{userId}/unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount(int userId)
    {
        try
        {
            _logger.LogInformation("Getting unread count for user {UserId}", userId);

            var count = await _notificationService.GetUnreadCount(userId);

            _logger.LogInformation("User {UserId} has {Count} unread notifications", userId, count);

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Unread count retrieved successfully",
                Data = count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get unread count for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<int>
            {
                Success = false,
                Message = "An error occurred while retrieving unread count",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // PUT /api/notifications/read/{id}
    [HttpPut("read/{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(int id)
    {
        try
        {
            _logger.LogInformation("Marking notification {NotificationId} as read", id);

            var result = await _notificationService.MarkAsRead(id);
            if (!result)
            {
                _logger.LogWarning("Notification {NotificationId} not found", id);
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Notification not found"
                });
            }

            _logger.LogInformation("Notification {NotificationId} marked as read successfully", id);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Notification marked as read successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while marking notification as read",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // PUT /api/notifications/read-all/{userId}
    [HttpPut("read-all/{userId}")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAllAsRead(int userId)
    {
        try
        {
            _logger.LogInformation("Marking all notifications as read for user {UserId}", userId);

            var result = await _notificationService.MarkAllRead(userId);
            if (!result)
            {
                _logger.LogWarning("Failed to mark all notifications as read for user {UserId}", userId);
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Failed to mark all notifications as read"
                });
            }

            _logger.LogInformation("All notifications marked as read for user {UserId}", userId);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "All notifications marked as read successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark all notifications as read for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while marking all notifications as read",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // DELETE /api/notifications/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteNotification(int id)
    {
        try
        {
            _logger.LogInformation("Deleting notification {NotificationId}", id);

            var result = await _notificationService.DeleteNotification(id);
            if (!result)
            {
                _logger.LogWarning("Notification {NotificationId} not found for deletion", id);
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Notification not found"
                });
            }

            _logger.LogInformation("Notification {NotificationId} deleted successfully", id);

            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Notification deleted successfully",
                Data = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete notification {NotificationId}", id);
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while deleting notification",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    private static NotificationResponseDto MapToNotificationResponseDto(Notification notification)
    {
        return new NotificationResponseDto
        {
            NotificationId = notification.NotificationId,
            UserId = notification.UserId,
            Type = notification.Type,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
