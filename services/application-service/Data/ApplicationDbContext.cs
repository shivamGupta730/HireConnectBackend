using HireConnect.ApplicationService.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.ApplicationService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Application> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("applications", "application");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JobId).HasColumnName("JobId");
            entity.Property(e => e.CandidateId).HasColumnName("CandidateId");
            entity.Property(e => e.Status).HasColumnName("Status");
            entity.Property(e => e.CoverLetter).HasColumnName("CoverLetter");
            entity.Property(e => e.ResumeUrl).HasColumnName("ResumeUrl");
            entity.Property(e => e.ExpectedSalary).HasColumnName("ExpectedSalary");
            entity.Property(e => e.Notes).HasColumnName("Notes");
            entity.Property(e => e.AppliedAt).HasColumnName("AppliedAt");
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
            entity.Property(e => e.StatusChangedAt).HasColumnName("StatusChangedAt");
            
            // Add unique constraint for JobId and CandidateId
            entity.HasIndex(e => new { e.JobId, e.CandidateId }).IsUnique();
        });
    }
}
