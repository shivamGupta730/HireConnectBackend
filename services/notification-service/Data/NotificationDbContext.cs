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
            entity.Property(e => e.NotificationId).HasColumnName("NotificationId");
            entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
            entity.Property(e => e.Type).HasColumnName("Type").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Message).HasColumnName("Message").IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsRead).HasColumnName("IsRead").HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Indexes for performance optimization
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Notifications_UserId");
            entity.HasIndex(e => new { e.UserId, e.IsRead }).HasDatabaseName("IX_Notifications_UserId_IsRead");
        });
    }
}
