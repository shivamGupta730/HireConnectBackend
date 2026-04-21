using HireConnect.AuthService.Dtos;
using HireConnect.Shared.Models;

namespace HireConnect.AuthService.Services;

public interface IGitHubOAuthService
{
    string GetAuthorizationUrl();
    Task<GitHubUserInfo?> ExchangeCodeForUserInfoAsync(string code);
    Task<LoginResponse> AuthenticateOrCreateUserAsync(GitHubUserInfo userInfo);
}
