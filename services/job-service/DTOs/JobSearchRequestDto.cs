using HireConnect.Shared.Models;

namespace HireConnect.JobService.DTOs;

public class JobSearchRequestDto : BaseRequest
{
    public string? Query { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? Type { get; set; }
    public bool? IsRemote { get; set; }
}
