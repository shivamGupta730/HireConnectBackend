using HireConnect.AuthService.Dtos;
using HireConnect.AuthService.Repositories;
using HireConnect.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace HireConnect.AuthService.Services;

public class GitHubOAuthService : IGitHubOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GitHubOAuthService> _logger;

    public GitHubOAuthService(
        HttpClient httpClient,
        IConfiguration configuration,
        IUserRepository userRepository,
        ILogger<GitHubOAuthService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _userRepository = userRepository;
        _logger = logger;
    }

    public string GetAuthorizationUrl()
    {
        var clientId = _configuration["GitHubAuth:ClientId"];
        var redirectUri = $"{_configuration["ApiBaseUrl"]}/api/auth/github/callback";
        var scope = "read:user user:email";
        var state = GenerateRandomString(32);

        return $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={state}";
    }

    public async Task<GitHubUserInfo?> ExchangeCodeForUserInfoAsync(string code)
    {
        try
        {
            // Exchange code for access token
            var tokenResponse = await ExchangeCodeForAccessTokenAsync(code);
            if (tokenResponse == null)
            {
                _logger.LogError("Failed to exchange code for access token");
                return null;
            }

            // Get user info
            var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);
            if (userInfo == null)
            {
                _logger.LogError("Failed to get user info from GitHub");
                return null;
            }

            // Get user emails to find primary email
            var emails = await GetUserEmailsAsync(tokenResponse.AccessToken);
            if (emails != null)
            {
                var primaryEmail = emails.FirstOrDefault(e => e.Primary && e.Verified);
                if (primaryEmail != null)
                {
                    userInfo.Email = primaryEmail.Email;
                }
            }

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging code for user info");
            return null;
        }
    }

    public async Task<LoginResponse> AuthenticateOrCreateUserAsync(GitHubUserInfo userInfo)
    {
        // Check if user exists by email
        var existingUser = await _userRepository.GetByEmailAsync(userInfo.Email);
        
        if (existingUser != null)
        {
            // User exists, generate JWT token
            return GenerateLoginResponse(existingUser);
        }

        // Create new user
        var newUser = new User
        {
            Email = userInfo.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Random password for OAuth users
            Role = UserRole.Candidate, // Default role
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(newUser);
        return GenerateLoginResponse(createdUser);
    }

    private async Task<GitHubTokenResponse?> ExchangeCodeForAccessTokenAsync(string code)
    {
        var clientId = _configuration["GitHubAuth:ClientId"];
        var clientSecret = _configuration["GitHubAuth:ClientSecret"];
        var redirectUri = $"{_configuration["ApiBaseUrl"]}/api/auth/github/callback";

        var requestBody = new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "code", code },
            { "redirect_uri", redirectUri }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubTokenResponse>(json);
    }

    private async Task<GitHubUserInfo?> GetUserInfoAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("User-Agent", "HireConnect-AuthService");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubUserInfo>(json);
    }

    private async Task<List<GitHubEmailInfo>?> GetUserEmailsAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("User-Agent", "HireConnect-AuthService");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<GitHubEmailInfo>>(json);
    }

    private LoginResponse GenerateLoginResponse(User user)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationMinutes")),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new LoginResponse
        {
            Token = tokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpirationMinutes")),
            Role = user.Role,
            UserId = user.Id
        };
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
}

public class GitHubTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;
}

public class GitHubUserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("login")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
}

public class GitHubEmailInfo
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }

    [JsonPropertyName("visibility")]
    public string? Visibility { get; set; }
}
