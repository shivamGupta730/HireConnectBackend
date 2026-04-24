using HireConnect.NotificationService.Data;
using HireConnect.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.NotificationService.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> FindByUserId(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> FindByUserIdAndIsRead(int userId, bool isRead)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead == isRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> CountByUserIdAndIsRead(int userId, bool isRead)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.IsRead == isRead);
    }

    public async Task<bool> DeleteByNotificationId(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
            return false;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Notification> CreateAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<Notification> UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
        return notification;
    }
}
