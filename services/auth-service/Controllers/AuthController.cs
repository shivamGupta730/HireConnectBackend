using HireConnect.AuthService.Dtos;
using HireConnect.AuthService.Services;
using HireConnect.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HireConnect.AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGitHubOAuthService _gitHubOAuthService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IGitHubOAuthService gitHubOAuthService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _gitHubOAuthService = gitHubOAuthService;
        _logger = logger;
    }

    // 🔐 REGISTER
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Register attempt for: {Email}", request.Email);

            var result = await _authService.RegisterAsync(request);

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Registration successful",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed");

            return BadRequest(new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // 🔐 LOGIN
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for: {Email}", request.Email);

            var result = await _authService.LoginAsync(request);

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");

            return BadRequest(new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // 👤 PROFILE (Protected)
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new ApiResponse<UserResponseDto>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var userId = int.Parse(userIdClaim);

            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new ApiResponse<UserResponseDto>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            return Ok(new ApiResponse<UserResponseDto>
            {
                Success = true,
                Message = "Profile retrieved successfully",
                Data = user
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get profile");

            return BadRequest(new ApiResponse<UserResponseDto>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // 🔑 TOKEN VALIDATE (Protected)
    [HttpGet("validate")]
    [Authorize]
    public IActionResult Validate()
    {
        return Ok(new ApiResponse<bool>
        {
            Success = true,
            Message = "Token is valid",
            Data = true
        });
    }

    // GITHUB OAUTH LOGIN
    [HttpGet("github/login")]
    [AllowAnonymous]
    public IActionResult GitHubLogin()
    {
        try
        {
            var authUrl = _gitHubOAuthService.GetAuthorizationUrl();
            
            return Redirect(authUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate GitHub authorization URL");
            
            return BadRequest(new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Failed to initiate GitHub login",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // GITHUB OAUTH CALLBACK
    [HttpGet("github/callback")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> GitHubCallback([FromQuery] string code, [FromQuery] string? state)
    {
        try
        {
            _logger.LogInformation("GitHub OAuth callback received with code: {Code}", code?.Substring(0, Math.Min(10, code?.Length ?? 0)) + "...");

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Authorization code is required",
                    Errors = new List<string> { "Missing authorization code" }
                });
            }

            var userInfo = await _gitHubOAuthService.ExchangeCodeForUserInfoAsync(code);
            if (userInfo == null)
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Failed to retrieve user information from GitHub",
                    Errors = new List<string> { "GitHub authentication failed" }
                });
            }

            if (string.IsNullOrEmpty(userInfo.Email))
            {
                return BadRequest(new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Email is required for authentication",
                    Errors = new List<string> { "GitHub account must have a verified email address" }
                });
            }

            var loginResponse = await _gitHubOAuthService.AuthenticateOrCreateUserAsync(userInfo);

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful",
                Data = loginResponse
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GitHub OAuth callback failed");
            
            return BadRequest(new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "GitHub authentication failed",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}