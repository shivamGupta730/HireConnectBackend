using HireConnect.Shared.Models;

namespace HireConnect.AuthService.Tests.Helpers;

public static class TestDataSeeder
{
    public static User CreateTestUser(int id = 1, string email = "test@test.com",
        string passwordHash = "$2a$11$fake", UserRole role = UserRole.Candidate)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static RegisterRequest CreateRegisterRequest(string email = "new@test.com",
        string password = "Test123!", UserRole role = UserRole.Candidate)
    {
        return new RegisterRequest
        {
            Email = email,
            Password = password,
            Role = role
        };
    }

    public static LoginRequest CreateLoginRequest(string email = "test@test.com",
        string password = "Test123!")
    {
        return new LoginRequest
        {
            Email = email,
            Password = password
        };
    }
}
