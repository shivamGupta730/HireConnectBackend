using HireConnect.NotificationService.Models;

namespace HireConnect.NotificationService.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> FindByUserId(int userId);
    Task<IEnumerable<Notification>> FindByUserIdAndIsRead(int userId, bool isRead);
    Task<int> CountByUserIdAndIsRead(int userId, bool isRead);
    Task<bool> DeleteByNotificationId(int id);
    Task<Notification> CreateAsync(Notification notification);
    Task<Notification> UpdateAsync(Notification notification);
}
