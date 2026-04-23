using HireConnect.JobService.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.JobService.Data;

public class JobDbContext : DbContext
{
    public JobDbContext(DbContextOptions<JobDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("jobs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("USD");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Requirements).HasMaxLength(2000);
            entity.Property(e => e.Benefits).HasMaxLength(2000);
            entity.Property(e => e.ExperienceRequired).HasMaxLength(1000);
            // PostedBy maps to UserId from Auth Service JWT - ensure correct column mapping
            entity.Property(e => e.PostedBy).HasColumnName("PostedBy");
            entity.Property(e => e.PostedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);
            entity.Property(e => e.ApplicationCount).HasDefaultValue(0);
            
            // Configure Skills as PostgreSQL array for string[]
            entity.Property(e => e.Skills)
                .HasColumnType("text[]")
                .HasConversion(
                    v => v,
                    v => v ?? Array.Empty<string>())
                .Metadata.SetValueComparer(
                    new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<string[]>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToArray()));
            
            });
    }
}
