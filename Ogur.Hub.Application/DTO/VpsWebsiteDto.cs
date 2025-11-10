// File: Ogur.Hub.Application/DTO/VpsWebsiteDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// Data transfer object for VPS website information.
/// </summary>
public record VpsWebsiteDto
{
    /// <summary>
    /// Gets the website identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the domain name.
    /// </summary>
    public string Domain { get; init; } = string.Empty;

    /// <summary>
    /// Gets the Traefik service name.
    /// </summary>
    public string ServiceName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the associated container name.
    /// </summary>
    public string? ContainerName { get; init; }

    /// <summary>
    /// Gets whether SSL/TLS is enabled.
    /// </summary>
    public bool SslEnabled { get; init; }

    /// <summary>
    /// Gets the SSL certificate expiration date.
    /// </summary>
    public DateTime? SslExpiresAt { get; init; }

    /// <summary>
    /// Gets whether the website is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Gets the timestamp when the website was last checked.
    /// </summary>
    public DateTime? LastCheckedAt { get; init; }

    /// <summary>
    /// Gets the HTTP status code from the last health check.
    /// </summary>
    public int? LastStatusCode { get; init; }

    /// <summary>
    /// Gets the response time in milliseconds from the last health check.
    /// </summary>
    public int? LastResponseTimeMs { get; init; }

    /// <summary>
    /// Gets whether the SSL certificate is expiring soon (within 30 days).
    /// </summary>
    public bool SslExpiringSoon { get; init; }
}