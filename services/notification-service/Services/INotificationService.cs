using HireConnect.NotificationService.Models;

namespace HireConnect.NotificationService.Services;

public interface INotificationService
{
    Task<Notification> SendNotification(Notification notification);
    Task<bool> MarkAsRead(int id);
    Task<bool> MarkAllRead(int userId);
    Task<IEnumerable<Notification>> GetByUser(int userId);
    Task<int> GetUnreadCount(int userId);
    Task<bool> DeleteNotification(int id);
    Task SendEmailAlert(string email, string message);
}
