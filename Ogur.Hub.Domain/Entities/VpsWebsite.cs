// File: Ogur.Hub.Domain/Entities/VpsWebsite.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a website/domain configured on the VPS (Traefik routes).
/// </summary>
public class VpsWebsite
{
    /// <summary>
    /// Gets or sets the website identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the domain name.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Traefik service name.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the container ID this website is routing to.
    /// </summary>
    public int? ContainerId { get; set; }

    /// <summary>
    /// Gets or sets the associated container.
    /// </summary>
    public VpsContainer? Container { get; set; }

    /// <summary>
    /// Gets or sets whether SSL/TLS is enabled.
    /// </summary>
    public bool SslEnabled { get; set; }

    /// <summary>
    /// Gets or sets the SSL certificate expiration date.
    /// </summary>
    public DateTime? SslExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets whether the website is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the website was last checked.
    /// </summary>
    public DateTime LastCheckedAt { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code from the last health check.
    /// </summary>
    public int? LastStatusCode { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds from the last health check.
    /// </summary>
    public int? LastResponseTimeMs { get; set; }
}