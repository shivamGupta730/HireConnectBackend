using HireConnect.ProfileService.Services;
using HireConnect.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HireConnect.ProfileService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    // GET PROFILE
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Get both candidate and recruiter profiles
        var candidateProfile = await _profileService.GetMyProfileAsync(userId);
        var recruiterProfile = await _profileService.GetMyRecruiterProfileAsync(userId);

        CandidateResponseDto? candidateResponseDto = null;
        RecruiterResponseDto? recruiterResponseDto = null;

        // Map candidate profile if exists
        if (candidateProfile != null)
        {
            candidateResponseDto = new CandidateResponseDto
            {
                id = candidateProfile.Id,
                fullName = candidateProfile.FullName,
                email = candidateProfile.Email,
                mobile = candidateProfile.Mobile,
                skills = string.Join(",", candidateProfile.Skills),
                experience = candidateProfile.Experience,
                education = candidateProfile.Education ?? "",
                resumeUrl = candidateProfile.ResumeUrl ?? ""
            };
        }

        // Map recruiter profile if exists
        if (recruiterProfile != null)
        {
            recruiterResponseDto = new RecruiterResponseDto
            {
                id = recruiterProfile.Id,
                fullName = recruiterProfile.FullName,
                companyName = recruiterProfile.CompanyName,
                industry = recruiterProfile.Industry ?? "",
                website = recruiterProfile.Website ?? "",
                description = recruiterProfile.Description ?? "",
                companySize = recruiterProfile.CompanySize ?? "",
                headquarters = recruiterProfile.Headquarters ?? ""
            };
        }

        return Ok(new
        {
            success = true,
            message = "Profile retrieved successfully",
            data = new
            {
                candidateProfile = candidateResponseDto,
                recruiterProfile = recruiterResponseDto
            }
        });
    }

    // POST /api/Profile/candidate
    [HttpPost("candidate")]
    public async Task<IActionResult> CreateCandidate([FromBody] RequestWrapper<CreateCandidateRequest> wrapperRequest)
    {
        if (wrapperRequest?.Request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid data"
            });
        }

        var request = wrapperRequest.Request;

        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Check if profile already exists
        var existingProfile = await _profileService.GetMyProfileAsync(userId);
        if (existingProfile != null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Profile already exists. Use update API."
            });
        }

        // Map DTO to entity
        var candidate = new Candidate
        {
            UserId = userId,
            FullName = request.fullName,
            Email = request.email,
            Mobile = request.mobile,
            Skills = string.IsNullOrWhiteSpace(request.skills) 
                ? new List<string>() 
                : request.skills.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Experience = request.experience,
            Education = request.education,
            ResumeUrl = request.resumeUrl,
            PortfolioUrl = request.portfolioUrl,
            LinkedInUrl = request.linkedInUrl,
            GitHubUrl = request.gitHubUrl,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _profileService.CreateCandidateAsync(candidate);

        // Map entity to response DTO
        var responseDto = new CandidateResponseDto
        {
            id = result.Id,
            fullName = result.FullName,
            email = result.Email,
            mobile = result.Mobile,
            skills = string.Join(",", result.Skills),
            experience = result.Experience,
            education = result.Education
        };

        return Ok(new
        {
            success = true,
            message = "Candidate profile created successfully",
            data = responseDto
        });
    }

    // POST /api/Profile/recruiter
    [HttpPost("recruiter")]
    public async Task<IActionResult> CreateRecruiter([FromBody] RequestWrapper<CreateRecruiterRequest> wrapperRequest)
    {
        if (wrapperRequest?.Request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid data"
            });
        }

        var request = wrapperRequest.Request;

        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Check if recruiter profile already exists
        var existingProfile = await _profileService.GetMyRecruiterProfileAsync(userId);
        if (existingProfile != null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Profile already exists. Use update API."
            });
        }

        // Map DTO to entity
        var recruiter = new Recruiter
        {
            UserId = userId,
            FullName = request.fullName,
            CompanyName = request.companyName,
            Industry = request.industry,
            Website = request.website,
            Description = request.description,
            CompanySize = request.companySize,
            Headquarters = request.headquarters,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _profileService.CreateRecruiterAsync(recruiter);

        // Map entity to response DTO
        var responseDto = new RecruiterResponseDto
        {
            id = result.Id,
            fullName = result.FullName,
            companyName = result.CompanyName,
            industry = result.Industry,
            website = result.Website,
            description = result.Description,
            companySize = result.CompanySize,
            headquarters = result.Headquarters
        };

        return Ok(new
        {
            success = true,
            message = "Recruiter profile created successfully",
            data = responseDto
        });
    }

    // PUT /api/Profile/candidate
    [HttpPut("candidate")]
    public async Task<IActionResult> UpdateCandidate([FromBody] CreateCandidateRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid data"
            });
        }

        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Fetch existing profile
        var existingProfile = await _profileService.GetMyProfileAsync(userId);
        if (existingProfile == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Profile not found"
            });
        }

        // Update fields from request
        existingProfile.FullName = request.fullName;
        existingProfile.Email = request.email;
        existingProfile.Mobile = request.mobile;
        existingProfile.Skills = string.IsNullOrWhiteSpace(request.skills) 
            ? new List<string>() 
            : request.skills.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s.Trim()).ToList();
        existingProfile.Experience = request.experience;
        existingProfile.Education = request.education;
        existingProfile.ResumeUrl = request.resumeUrl;
        existingProfile.PortfolioUrl = request.portfolioUrl;
        existingProfile.LinkedInUrl = request.linkedInUrl;
        existingProfile.GitHubUrl = request.gitHubUrl;
        existingProfile.UpdatedAt = DateTime.UtcNow;

        var result = await _profileService.UpdateCandidateAsync(existingProfile);

        // Map entity to response DTO
        var responseDto = new CandidateResponseDto
        {
            id = result.Id,
            fullName = result.FullName,
            email = result.Email,
            mobile = result.Mobile,
            skills = string.Join(",", result.Skills),
            experience = result.Experience,
            education = result.Education
        };

        return Ok(new
        {
            success = true,
            message = "Candidate profile updated successfully",
            data = responseDto
        });
    }

    // PUT /api/Profile/recruiter
    [HttpPut("recruiter")]
    public async Task<IActionResult> UpdateRecruiter([FromBody] CreateRecruiterRequest request)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid data"
            });
        }

        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Fetch existing recruiter profile
        var existingProfile = await _profileService.GetMyRecruiterProfileAsync(userId);
        if (existingProfile == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Profile not found"
            });
        }

        // Update fields from request
        existingProfile.FullName = request.fullName;
        existingProfile.CompanyName = request.companyName;
        existingProfile.Industry = request.industry;
        existingProfile.Website = request.website;
        existingProfile.Description = request.description;
        existingProfile.CompanySize = request.companySize;
        existingProfile.Headquarters = request.headquarters;
        existingProfile.UpdatedAt = DateTime.UtcNow;

        var result = await _profileService.UpdateRecruiterAsync(existingProfile);

        // Map entity to response DTO
        var responseDto = new RecruiterResponseDto
        {
            id = result.Id,
            fullName = result.FullName,
            companyName = result.CompanyName,
            industry = result.Industry,
            website = result.Website,
            description = result.Description,
            companySize = result.CompanySize,
            headquarters = result.Headquarters
        };

        return Ok(new
        {
            success = true,
            message = "Recruiter profile updated successfully",
            data = responseDto
        });
    }

    // DELETE /api/Profile/candidate
    [HttpDelete("candidate")]
    public async Task<IActionResult> DeleteCandidate()
    {
        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Delete profile using repository
        var result = await _profileService.DeleteCandidateAsync(userId);

        if (!result)
        {
            return NotFound(new
            {
                success = false,
                message = "Profile not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Candidate profile deleted successfully",
            data = (object?)null
        });
    }

    // DELETE /api/Profile/recruiter
    [HttpDelete("recruiter")]
    public async Task<IActionResult> DeleteRecruiter()
    {
        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Delete recruiter profile using service
        var result = await _profileService.DeleteRecruiterAsync(userId);

        if (!result)
        {
            return NotFound(new
            {
                success = false,
                message = "Profile not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Recruiter profile deleted successfully",
            data = (object?)null
        });
    }

    // POST /api/Profile/upload-resume
    [HttpPost("upload-resume")]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        // Validate file
        if (file == null || file.Length == 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "No file uploaded"
            });
        }

        // Validate file extension
        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
        {
            return BadRequest(new
            {
                success = false,
                message = "Only PDF files are allowed"
            });
        }

        // Extract userId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "User not authenticated"
            });
        }

        int userId = int.Parse(userIdClaim);

        // Create uploads directory if not exists
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "resumes");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}.pdf";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Get existing candidate profile
        var existingProfile = await _profileService.GetMyProfileAsync(userId);
        if (existingProfile == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Candidate profile not found. Create profile first."
            });
        }

        // Update resume URL in database
        existingProfile.ResumeUrl = $"http://localhost:5001/uploads/resumes/{fileName}";
        existingProfile.UpdatedAt = DateTime.UtcNow;

        var updatedProfile = await _profileService.UpdateCandidateAsync(existingProfile);

        return Ok(new
        {
            success = true,
            message = "Resume uploaded successfully",
            data = new
            {
                resumeUrl = existingProfile.ResumeUrl
            }
        });
    }
}