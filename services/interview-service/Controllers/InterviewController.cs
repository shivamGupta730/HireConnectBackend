using HireConnect.InterviewService.Services;
using HireConnect.InterviewService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HireConnect.InterviewService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterviewController : ControllerBase
{
    private readonly IInterviewService _interviewService;
    private readonly ILogger<InterviewController> _logger;

    public InterviewController(IInterviewService interviewService, ILogger<InterviewController> logger)
    {
        _interviewService = interviewService;
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

    // POST /api/Interview - Schedule Interview
    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    public async Task<ActionResult<InterviewResponseDto>> ScheduleInterview([FromBody] ScheduleInterviewDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var recruiterId = GetCurrentUserId();
            var interview = await _interviewService.ScheduleInterviewAsync(recruiterId, dto);
            return CreatedAtAction(nameof(GetInterview), new { id = interview.Id }, interview);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while scheduling interview: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while scheduling interview: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule interview");
            return BadRequest(new { success = false, message = "An error occurred while scheduling the interview" });
        }
    }

    // GET /api/Interview/my - Get My Interviews (Candidate)
    [HttpGet("my")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<PagedResponse<InterviewResponseDto>>> GetMyInterviews([FromQuery] BaseRequest request)
    {
        try
        {
            var candidateId = GetCurrentUserId();
            var interviews = await _interviewService.GetMyInterviewsAsync(candidateId, request);
            return Ok(interviews);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting my interviews: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while getting my interviews: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get my interviews");
            return BadRequest(new { success = false, message = "An error occurred while retrieving interviews" });
        }
    }

    // GET /api/Interview/job/{jobId} - Get Interviews by Job (Recruiter)
    [HttpGet("job/{jobId}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<ActionResult<PagedResponse<InterviewResponseDto>>> GetInterviewsByJob(int jobId, [FromQuery] BaseRequest request)
    {
        try
        {
            var recruiterId = GetCurrentUserId();
            var interviews = await _interviewService.GetInterviewsByJobAsync(jobId, recruiterId, request);
            return Ok(interviews);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting interviews by job: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while getting interviews by job: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get interviews by job");
            return BadRequest(new { success = false, message = "An error occurred while retrieving job interviews" });
        }
    }

    // GET /api/Interview/{id} - Get Interview by Id
    [HttpGet("{id}")]
    public async Task<ActionResult<InterviewResponseDto>> GetInterview(int id)
    {
        try
        {
            var interview = await _interviewService.GetByIdAsync(id);
            if (interview == null)
            {
                return NotFound();
            }
            return Ok(interview);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting interview: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while getting interview: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get interview");
            return BadRequest(new { success = false, message = "An error occurred while retrieving interview" });
        }
    }

    // PUT /api/Interview/{id} - Update Interview Status
    [HttpPut("{id}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> UpdateInterviewStatus(int id, [FromBody] UpdateInterviewStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var recruiterId = GetCurrentUserId();
            var result = await _interviewService.UpdateInterviewStatusAsync(id, recruiterId, dto);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating interview status: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while updating interview status: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update interview status");
            return BadRequest(new { success = false, message = "An error occurred while updating interview status" });
        }
    }

    // DELETE /api/Interview/{id} - Cancel Interview
    [HttpDelete("{id}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> CancelInterview(int id)
    {
        try
        {
            var recruiterId = GetCurrentUserId();
            var result = await _interviewService.CancelInterviewAsync(id, recruiterId);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while cancelling interview: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while cancelling interview: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel interview");
            return BadRequest(new { success = false, message = "An error occurred while cancelling interview" });
        }
    }
}
