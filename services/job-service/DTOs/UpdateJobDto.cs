using System.ComponentModel.DataAnnotations;

namespace HireConnect.JobService.DTOs;

public class UpdateJobDto
{
    [StringLength(255, ErrorMessage = "Job title cannot exceed 255 characters")]
    public string? Title { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }

    public string? Type { get; set; }

    [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
    public string? Location { get; set; }

    public bool? IsRemote { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Minimum salary must be greater than 0")]
    public decimal? SalaryMin { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Maximum salary must be greater than 0")]
    public decimal? SalaryMax { get; set; }

    [StringLength(10, ErrorMessage = "Currency cannot exceed 10 characters")]
    public string? Currency { get; set; }

    public List<string>? Skills { get; set; }

    [Range(0, 50, ErrorMessage = "Experience required must be between 0 and 50 years")]
    public int? ExperienceRequired { get; set; }

    public string? Description { get; set; }

    [StringLength(2000, ErrorMessage = "Requirements cannot exceed 2000 characters")]
    public string? Requirements { get; set; }

    [StringLength(2000, ErrorMessage = "Benefits cannot exceed 2000 characters")]
    public string? Benefits { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
