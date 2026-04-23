using HireConnect.JobService.Services;
using HireConnect.JobService.DTOs;
using HireConnect.JobService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HireConnect.JobService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobController> _logger;

    public JobController(IJobService jobService, ILogger<JobController> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    // Always use UserId as identity across services - extract from JWT token
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return int.Parse(userIdClaim);
    }

    // POST /api/jobs (Recruiter)
    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    public async Task<ActionResult<JobResponseDto>> CreateJob([FromBody] CreateJobDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var recruiterId = GetCurrentUserId();
        var job = await _jobService.CreateJobAsync(recruiterId, dto);
        
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    // GET /api/jobs (Public - only open jobs)
    [HttpGet]
    public async Task<ActionResult<PagedResponse<JobResponseDto>>> GetJobs([FromQuery] BaseRequest dto)
    {
        var jobs = await _jobService.GetOpenJobsAsync(dto);
        return Ok(jobs);
    }

    // GET /api/jobs/{id} (Public)
    [HttpGet("{id}")]
    public async Task<ActionResult<JobResponseDto>> GetJob(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        return Ok(job);
    }

    // GET /api/jobs/search (Public)
    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<JobResponseDto>>> SearchJobs([FromQuery] JobSearchRequestDto dto)
    {
        var jobs = await _jobService.SearchJobsAsync(dto);
        return Ok(jobs);
    }

    // GET /api/jobs/recruiter/{id} (Recruiter jobs)
    [HttpGet("recruiter/{recruiterId}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<ActionResult<PagedResponse<JobResponseDto>>> GetJobsByRecruiter(int recruiterId, [FromQuery] BaseRequest dto)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId != recruiterId)
        {
            return Forbid();
        }

        var jobs = await _jobService.GetJobsByRecruiterAsync(recruiterId, dto);
        return Ok(jobs);
    }

    // PUT /api/jobs/{id} (Recruiter)
    [HttpPut("{id}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<ActionResult<JobResponseDto>> UpdateJob(int id, [FromBody] UpdateJobDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var recruiterId = GetCurrentUserId();
        var job = await _jobService.UpdateJobAsync(id, recruiterId, dto);
        
        if (job == null)
        {
            return NotFound();
        }

        return Ok(job);
    }

    // DELETE /api/jobs/{id} (Recruiter)
    [HttpDelete("{id}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var recruiterId = GetCurrentUserId();
        var result = await _jobService.DeleteJobAsync(id, recruiterId);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    // PATCH /api/jobs/{id}/status (Open/Closed)
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> UpdateJobStatus(int id, [FromBody] UpdateJobStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var recruiterId = GetCurrentUserId();
        var result = await _jobService.UpdateJobStatusAsync(id, recruiterId, dto.Status);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
