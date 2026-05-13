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
        modelBuilder.HasDefaultSchema("profile");

        modelBuilder.Entity<Candidate>(entity =>
        {
            entity.ToTable("candidates", "profile");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FullName).HasColumnName("fullname").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Mobile).HasColumnName("mobile").HasMaxLength(20);
            entity.Property(e => e.Skills).HasColumnName("skills").HasColumnType("text[]");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.ExperienceYears).HasColumnName("experienceyears");
            entity.Property(e => e.Education).HasColumnName("education");
            entity.Property(e => e.ResumeUrl).HasColumnName("resumeurl");
            entity.Property(e => e.GitHubUrl).HasColumnName("githuburl");
            entity.Property(e => e.LinkedInUrl).HasColumnName("linkedinurl");
            entity.Property(e => e.PortfolioUrl).HasColumnName("portfoliourl");
            entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        modelBuilder.Entity<Recruiter>(entity =>
        {
            entity.ToTable("recruiters", "profile");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FullName).HasColumnName("fullname").IsRequired().HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasColumnName("companyname").IsRequired().HasMaxLength(255);
            entity.Property(e => e.Designation).HasColumnName("designation").HasMaxLength(100);
            entity.Property(e => e.Industry).HasColumnName("industry").HasMaxLength(100);
            entity.Property(e => e.Website).HasColumnName("website").HasMaxLength(500);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CompanySize).HasColumnName("companysize").HasMaxLength(50);
            entity.Property(e => e.Headquarters).HasColumnName("headquarters").HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");
            entity.HasIndex(e => e.UserId).IsUnique();
        });
    }
}