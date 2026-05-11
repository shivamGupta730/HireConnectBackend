using FluentAssertions;
using HireConnect.JobService.Data;
using HireConnect.JobService.Models;
using HireConnect.JobService.Repositories;
using HireConnect.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HireConnect.JobService.Tests.Helpers;

/// <summary>
/// Test-specific DbContext that removes PostgreSQL-specific configurations
/// (like text[] column type) which are incompatible with InMemory provider.
/// </summary>
public class TestJobDbContext : JobDbContext
{
    public TestJobDbContext(DbContextOptions<JobDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("jobs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("USD");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.PostedBy).HasColumnName("PostedBy");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);
            entity.Property(e => e.ApplicationCount).HasDefaultValue(0);
            // Skip text[] and CURRENT_TIMESTAMP for InMemory compatibility
        });
    }
}
