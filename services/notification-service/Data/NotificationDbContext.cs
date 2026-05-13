using HireConnect.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notifications", "notification");
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.NotificationId).HasColumnName("notificationid");
            entity.Property(e => e.UserId).HasColumnName("userid").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).HasColumnName("message").IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsRead).HasColumnName("isread").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes for performance optimization
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Notifications_UserId");
            entity.HasIndex(e => new { e.UserId, e.IsRead }).HasDatabaseName("IX_Notifications_UserId_IsRead");
        });
    }
}
