using HireConnect.NotificationService.Repositories;
using HireConnect.NotificationService.Models;
using Microsoft.Extensions.Logging;

namespace HireConnect.NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<Notification> SendNotification(Notification notification)
    {
        try
        {
            var createdNotification = await _notificationRepository.CreateAsync(notification);
            _logger.LogInformation("Notification created successfully for user {UserId}", notification.UserId);
            return createdNotification;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification for user {UserId}", notification.UserId);
            throw;
        }
    }

    public async Task<bool> MarkAsRead(int id)
    {
        try
        {
            // Get all unread notifications to find the one to mark as read
            var allNotifications = await _notificationRepository.FindByUserIdAndIsRead(0, false);
            var notification = allNotifications.FirstOrDefault(n => n.NotificationId == id);
            
            if (notification == null)
            {
                _logger.LogWarning("Notification with ID {NotificationId} not found", id);
                return false;
            }

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            
            _logger.LogInformation("Notification {NotificationId} marked as read", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", id);
            return false;
        }
    }

    public async Task<bool> MarkAllRead(int userId)
    {
        try
        {
            var unreadNotifications = await _notificationRepository.FindByUserIdAndIsRead(userId, false);
            
            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);
            }
            
            _logger.LogInformation("Marked all notifications as read for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark all notifications as read for user {UserId}", userId);
            return false;
        }
    }

    public async Task<IEnumerable<Notification>> GetByUser(int userId)
    {
        try
        {
            var notifications = await _notificationRepository.FindByUserId(userId);
            _logger.LogInformation("Retrieved {Count} notifications for user {UserId}", notifications.Count(), userId);
            return notifications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve notifications for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetUnreadCount(int userId)
    {
        try
        {
            var count = await _notificationRepository.CountByUserIdAndIsRead(userId, false);
            _logger.LogInformation("User {UserId} has {Count} unread notifications", userId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get unread count for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> DeleteNotification(int id)
    {
        try
        {
            var result = await _notificationRepository.DeleteByNotificationId(id);
            if (result)
            {
                _logger.LogInformation("Notification {NotificationId} deleted successfully", id);
            }
            else
            {
                _logger.LogWarning("Notification {NotificationId} not found for deletion", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete notification {NotificationId}", id);
            return false;
        }
    }

    public async Task SendEmailAlert(string email, string message)
    {
        try
        {
            // SendGrid placeholder implementation
            // In production, this would integrate with SendGrid API
            _logger.LogInformation("Email alert placeholder: Sending to {Email} with message: {Message}", email, message);
            
            // TODO: Implement actual SendGrid integration
            // var apiKey = _configuration["SendGrid:ApiKey"];
            // var client = new SendGridClient(apiKey);
            // var from = new EmailAddress("noreply@hireconnect.com", "HireConnect");
            // var subject = "HireConnect Notification";
            // var to = new EmailAddress(email);
            // var plainTextContent = message;
            // var htmlContent = $"<p>{message}</p>";
            // var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            // var response = await client.SendEmailAsync(msg);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email alert to {Email}", email);
            throw;
        }
    }
}
