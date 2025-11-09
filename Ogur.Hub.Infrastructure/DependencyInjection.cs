// File: Ogur.Hub.Infrastructure/DependencyInjection.cs
// Project: Ogur.Hub.Infrastructure
// Namespace: Ogur.Hub.Infrastructure

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Infrastructure.BackgroundServices;
using Ogur.Hub.Infrastructure.Persistence;
using Ogur.Hub.Infrastructure.Persistence.Repositories;
using Ogur.Hub.Infrastructure.Services;
using Ogur.Hub.Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;



namespace Ogur.Hub.Infrastructure;

/// <summary>
/// Extension methods for registering infrastructure services in dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure layer services including database, repositories, and SignalR.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddSignalRServices();
        services.AddBackgroundServices();
        services.AddInfrastructureServices();

        return services;
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(
                connectionString,
                //ServerVersion.AutoDetect(connectionString),
                new MariaDbServerVersion(new Version(11, 2, 0)),
                mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });

            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddSignalRServices(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = false;
            options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        });
    }

    private static void AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<CommandProcessingService>();
    }

    private static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
    }
}