using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace HireConnect.ProfileService.Data;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
    {
    }

    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Recruiter> Recruiters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.ToTable("candidates");
            entity.HasKey(e => e.Id);
            // Always use UserId as identity across services - ensure uniqueness
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Skills).HasColumnType("text[]");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Recruiter>(entity =>
        {
            entity.ToTable("recruiters");
            entity.HasKey(e => e.Id);
            // Always use UserId as identity across services - ensure uniqueness
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}