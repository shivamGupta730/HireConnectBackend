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
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JobId).HasColumnName("jobid");
            entity.Property(e => e.CandidateId).HasColumnName("candidateid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CoverLetter).HasColumnName("coverletter");
            entity.Property(e => e.ResumeUrl).HasColumnName("resumeurl");
            entity.Property(e => e.ExpectedSalary).HasColumnName("expectedsalary");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.AppliedAt).HasColumnName("appliedat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
            entity.Property(e => e.StatusChangedAt).HasColumnName("statuschangedat");
            
            // Add unique constraint for JobId and CandidateId
            entity.HasIndex(e => new { e.JobId, e.CandidateId }).IsUnique();
        });
    }
}
