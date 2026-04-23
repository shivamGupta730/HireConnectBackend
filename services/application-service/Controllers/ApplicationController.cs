using HireConnect.ApplicationService.Services;
using HireConnect.ApplicationService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HireConnect.ApplicationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(IApplicationService applicationService, ILogger<ApplicationController> logger)
    {
        _applicationService = applicationService;
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

    // POST /api/application
    [HttpPost]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<ApplicationResponseDto>> CreateApplication([FromBody] CreateApplicationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var candidateId = GetCurrentUserId();
            var application = await _applicationService.CreateApplicationAsync(candidateId, dto);
            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating application: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while creating application: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create application");
            return BadRequest(new { success = false, message = "An error occurred while creating the application" });
        }
    }

    // GET /api/application/my
    [HttpGet("my")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<PagedResponse<ApplicationResponseDto>>> GetMyApplications([FromQuery] BaseRequest request)
    {
        try
        {
            var candidateId = GetCurrentUserId();
            var applications = await _applicationService.GetApplicationsByCandidateAsync(candidateId, request);
            return Ok(applications);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting my applications: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while getting my applications: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get my applications");
            return BadRequest(new { success = false, message = "An error occurred while retrieving applications" });
        }
    }

    // GET /api/application/job/{jobId}
    [HttpGet("job/{jobId}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<ActionResult<PagedResponse<ApplicationResponseDto>>> GetApplicationsByJob(int jobId, [FromQuery] BaseRequest request)
    {
        try
        {
            var recruiterId = GetCurrentUserId();
            var applications = await _applicationService.GetApplicationsByJobAsync(jobId, recruiterId, request);
            return Ok(applications);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting applications by job: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while getting applications by job: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get applications by job");
            return BadRequest(new { success = false, message = "An error occurred while retrieving job applications" });
        }
    }

    // GET /api/application/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationResponseDto>> GetApplication(int id)
    {
        try
        {
            var application = await _applicationService.GetByIdAsync(id);
            if (application == null)
            {
                return NotFound();
            }
            return Ok(application);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while getting application: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while getting application: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get application");
            return BadRequest(new { success = false, message = "An error occurred while retrieving application" });
        }
    }

    // PUT /api/application/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> UpdateApplicationStatus(int id, [FromBody] UpdateApplicationStatusDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var recruiterId = GetCurrentUserId();
            var result = await _applicationService.UpdateApplicationStatusAsync(id, recruiterId, dto);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating application status: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while updating application status: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update application status");
            return BadRequest(new { success = false, message = "An error occurred while updating application status" });
        }
    }

    // DELETE /api/application/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Candidate")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        try
        {
            var candidateId = GetCurrentUserId();
            var result = await _applicationService.DeleteApplicationAsync(id, candidateId);
            
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting application: {Message}", ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access while deleting application: {Message}", ex.Message);
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete application");
            return BadRequest(new { success = false, message = "An error occurred while deleting application" });
        }
    }
}
