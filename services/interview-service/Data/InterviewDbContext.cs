using HireConnect.InterviewService.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.InterviewService.Data;

public class InterviewDbContext : DbContext
{
    public InterviewDbContext(DbContextOptions<InterviewDbContext> options) : base(options)
    {
    }

    public DbSet<Interview> Interviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.ToTable("interviews", "interview");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationId");
            entity.Property(e => e.JobId).HasColumnName("JobId");
            entity.Property(e => e.CandidateId).HasColumnName("CandidateId");
            entity.Property(e => e.ScheduledAt).HasColumnName("ScheduledAt");
            entity.Property(e => e.MeetingLink).HasColumnName("MeetingLink").HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("Status");
            entity.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
            entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
        });
    }
}
