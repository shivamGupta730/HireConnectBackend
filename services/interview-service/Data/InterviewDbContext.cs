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
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("applicationid");
            entity.Property(e => e.JobId).HasColumnName("jobid");
            entity.Property(e => e.CandidateId).HasColumnName("candidateid");
            entity.Property(e => e.ScheduledAt).HasColumnName("scheduledat");
            entity.Property(e => e.MeetingLink).HasColumnName("meetinglink").HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("createdat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
        });
    }
}
