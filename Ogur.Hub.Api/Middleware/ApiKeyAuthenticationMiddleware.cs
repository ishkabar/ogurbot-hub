// File: Ogur.Hub.Api/Middleware/ApiKeyAuthenticationMiddleware.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Middleware

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.ValueObjects;
using System.Net;
using System.Text.Json;
using Ogur.Hub.Api.Models.Responses;
using DomainApplication = Ogur.Hub.Domain.Entities.Application;

namespace Ogur.Hub.Api.Middleware;

/// <summary>
/// Middleware for authenticating requests using API keys.
/// </summary>
public sealed class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;
    private const string ApiKeyHeaderName = "X-Api-Key";

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next middleware in the pipeline.</param>
    /// <param name="logger">Logger instance.</param>
    public ApiKeyAuthenticationMiddleware(RequestDelegate next, ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">HTTP context.</param>
    /// <param name="repository">Application repository.</param>
    public async Task InvokeAsync(HttpContext context, IRepository<DomainApplication, int> repository)
    {
        if (!RequiresApiKeyAuthentication(context))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader))
        {
            await WriteUnauthorizedResponse(context, "API key is missing");
            return;
        }

        var apiKey = apiKeyHeader.ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            await WriteUnauthorizedResponse(context, "API key is empty");
            return;
        }

        var applications = await repository.GetAllAsync(CancellationToken.None);
        var application = applications.FirstOrDefault(a => a.IsActive && a.VerifyApiKey(apiKey));

        if (application is null)
        {
            _logger.LogWarning("Invalid API key attempt from {IpAddress}", context.Connection.RemoteIpAddress);
            await WriteUnauthorizedResponse(context, "Invalid API key");
            return;
        }

        context.Items["ApplicationId"] = application.Id;
        context.Items["ApplicationName"] = application.Name;

        await _next(context);
    }

    private static bool RequiresApiKeyAuthentication(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        return path.StartsWith("/api/licenses/validate") ||
               path.StartsWith("/api/updates/check") ||
               path.StartsWith("/api/telemetry");
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var response = new ErrorResponse
        {
            Message = message,
            Code = "ApiKeyAuthenticationFailed"
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}