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
        modelBuilder.HasDefaultSchema("job");

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("jobs", "job");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Category).HasColumnName("category").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Location).HasColumnName("location").IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsRemote).HasColumnName("isremote");
            entity.Property(e => e.SalaryMin).HasColumnName("salarymin");
            entity.Property(e => e.SalaryMax).HasColumnName("salarymax");
            entity.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(10).HasDefaultValue("USD");
            entity.Property(e => e.Skills).HasColumnName("skills").HasColumnType("text[]");
            entity.Property(e => e.ExperienceRequired).HasColumnName("experiencerequired");
            entity.Property(e => e.Description).HasColumnName("description").IsRequired();
            entity.Property(e => e.Requirements).HasColumnName("requirements").HasMaxLength(2000);
            entity.Property(e => e.Benefits).HasColumnName("benefits").HasMaxLength(2000);
            entity.Property(e => e.PostedBy).HasColumnName("postedby");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.PostedAt).HasColumnName("postedat").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ExpiresAt).HasColumnName("expiresat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
            entity.Property(e => e.ViewCount).HasColumnName("viewcount").HasDefaultValue(0);
            entity.Property(e => e.ApplicationCount).HasColumnName("applicationcount").HasDefaultValue(0);
            
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
