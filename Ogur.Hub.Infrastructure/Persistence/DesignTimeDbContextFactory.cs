// File: Ogur.Hub.Infrastructure/Persistence/DesignTimeDbContextFactory.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure.Persistence

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ogur.Hub.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating ApplicationDbContext during migrations.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a new instance of ApplicationDbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command line arguments.</param>
    /// <returns>Configured ApplicationDbContext instance.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = "Server=localhost;Port=3307;Database=ogurhub;User=ogurhub;Password=OgurHubDb2024;AllowUserVariables=true;UseAffectedRows=false;";

        optionsBuilder.UseMySql(
            connectionString,
            new MariaDbServerVersion(new Version(11, 2, 0)),
            mySqlOptions =>
            {
                mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}