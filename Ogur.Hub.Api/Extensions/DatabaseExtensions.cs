// File: Ogur.Hub.Api/Extensions/DatabaseExtensions.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Extensions

using Microsoft.EntityFrameworkCore;
using Ogur.Hub.Infrastructure.Persistence;

namespace Ogur.Hub.Api.Extensions;

/// <summary>
/// Extension methods for database operations.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Applies pending migrations and ensures database is created.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns>Application builder for chaining.</returns>
    public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting database migration...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }

        return app;
    }
}