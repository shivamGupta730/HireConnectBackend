using Microsoft.EntityFrameworkCore;
using HireConnect.AuthService.Data;

namespace HireConnect.AuthService.Tests.Helpers;

public static class DbContextFactory
{
    public static AuthDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AuthDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
