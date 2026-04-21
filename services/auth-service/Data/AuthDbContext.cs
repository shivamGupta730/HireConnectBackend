using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.AuthService.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔥 IMPORTANT LINE
        modelBuilder.HasDefaultSchema("auth");

       modelBuilder.Entity<User>(entity =>
{
    entity.ToTable("users");

    entity.HasKey(e => e.Id);

    entity.Property(e => e.Id).HasColumnName("id");

    entity.Property(e => e.Email)
          .HasColumnName("email")
          .IsRequired()
          .HasMaxLength(255);

    entity.Property(e => e.PasswordHash)
          .HasColumnName("passwordhash")
          .IsRequired()
          .HasMaxLength(255);

    entity.Property(e => e.Role)
          .HasColumnName("role")
          .IsRequired();

    entity.Property(e => e.CreatedAt)
          .HasColumnName("createdat")
          .HasDefaultValueSql("CURRENT_TIMESTAMP");

    entity.Property(e => e.UpdatedAt)
          .HasColumnName("updatedat");

    entity.HasIndex(e => e.Email).IsUnique();
});
    }
}