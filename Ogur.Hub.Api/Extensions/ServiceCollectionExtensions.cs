// File: Ogur.Hub.Api/Extensions/ServiceCollectionExtensions.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Extensions

using Ogur.Hub.Api.Middleware;
using Ogur.Hub.Api.Services;

namespace Ogur.Hub.Api.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to register API-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds API layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}