// File: Hub.Application/DependencyInjection.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application

using Microsoft.Extensions.DependencyInjection;
using Ogur.Hub.Application.Commands.ApplicationsCommands;
using Ogur.Hub.Application.Commands.DevicesCommands;
using Ogur.Hub.Application.Commands.LicensesCommands;
using Ogur.Hub.Application.Commands.TelemetryCommands;
using Ogur.Hub.Application.Commands.UsersCommands;
using Ogur.Hub.Application.Queries.Applications;
using Ogur.Hub.Application.Queries.Dashboard;
using Ogur.Hub.Application.Queries.Devices;
using Ogur.Hub.Application.Queries.Licenses;
using Ogur.Hub.Application.Queries.Telemetry;
using Ogur.Hub.Application.Queries.Updates;
using Ogur.Hub.Application.Queries.Users;

namespace Ogur.Hub.Application;


/// <summary>
/// Dependency injection configuration for Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // Register Query Handlers
        services.AddScoped<GetApplicationsQueryHandler>();
        services.AddScoped<GetDashboardStatsQueryHandler>();
        services.AddScoped<GetDevicesQueryHandler>();
        services.AddScoped<GetLicensesQueryHandler>();
        services.AddScoped<GetTelemetryQueryHandler>();
        services.AddScoped<CheckForUpdatesQueryHandler>();
        services.AddScoped<GetUsersQueryHandler>();
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<RegisterUserCommandHandler>();

        // Register Command Handlers
        services.AddScoped<CreateApplicationCommandHandler>();
        services.AddScoped<UpdateApplicationCommandHandler>();
        services.AddScoped<BlockDeviceCommandHandler>();
        services.AddScoped<UnblockDeviceCommandHandler>();
        services.AddScoped<SendDeviceCommandCommandHandler>();
        services.AddScoped<UpdateDeviceCommandHandler>();
        services.AddScoped<AssignUserToDeviceCommandHandler>();
        services.AddScoped<RemoveUserFromDeviceCommandHandler>();
        services.AddScoped<CreateLicenseCommandHandler>();
        services.AddScoped<RevokeLicenseCommandHandler>();
        services.AddScoped<ValidateLicenseCommandHandler>();
        services.AddScoped<ReceiveTelemetryCommandHandler>();
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<UpdateUserCommandHandler>();
        services.AddScoped<DeleteUserCommandHandler>();

        return services;
    }
}