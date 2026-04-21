using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HireConnect.ProfileService.Data;

public class ProfileDbContext : DbContext
{
    public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
    {
    }

    public DbSet<CandidateProfile> CandidateProfiles { get; set; }
    public DbSet<RecruiterProfile> RecruiterProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("profile");

        modelBuilder.Entity<CandidateProfile>(entity =>
        {
            // ✅ Correct table name (IMPORTANT)
            entity.ToTable("candidateprofiles");

            // ✅ Correct column mappings
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName).HasColumnName("fullname");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Mobile).HasColumnName("mobile");
            entity.Property(e => e.Skills).HasColumnName("skills");
            entity.Property(e => e.Experience).HasColumnName("experience");
            entity.Property(e => e.Education).HasColumnName("education");
            entity.Property(e => e.ResumeUrl).HasColumnName("resumeurl");
            entity.Property(e => e.PortfolioUrl).HasColumnName("portfoliourl");
            entity.Property(e => e.LinkedInUrl).HasColumnName("linkedinurl");
            entity.Property(e => e.GitHubUrl).HasColumnName("githuburl");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.CreatedAt).HasColumnName("createdat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");

            // ❌ Ignore unwanted / non-existing properties
            entity.Ignore("AddressId");
            entity.Ignore("AddressId1");
            entity.Ignore("ExperienceYears");
        });

        modelBuilder.Entity<RecruiterProfile>(entity =>
        {
            entity.ToTable("recruiterprofiles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName).HasColumnName("fullname");
            entity.Property(e => e.CompanyName).HasColumnName("companyname");
            entity.Property(e => e.Industry).HasColumnName("industry");
            entity.Property(e => e.Website).HasColumnName("website");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CompanySize).HasColumnName("companysize");
            entity.Property(e => e.Headquarters).HasColumnName("headquarters");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.CreatedAt).HasColumnName("createdat");
            entity.Property(e => e.UpdatedAt).HasColumnName("updatedat");

            // Ignore properties not present in DB
            entity.Ignore(e => e.Designation);
            entity.Ignore(e => e.Address);
            entity.Ignore(e => e.User);
        });
    }
}