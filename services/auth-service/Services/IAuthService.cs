using HireConnect.AuthService.Dtos;
using HireConnect.Shared.Models;

namespace HireConnect.AuthService.Services;

public interface IAuthService
{
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<bool> ValidateTokenAsync(string token);
}
