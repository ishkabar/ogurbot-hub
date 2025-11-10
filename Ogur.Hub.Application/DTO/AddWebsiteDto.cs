// File: Ogur.Hub.Application/DTO/AddWebsiteDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// DTO for adding a new website to monitor.
/// </summary>
public sealed record AddWebsiteDto
{
    /// <summary>
    /// Domain name.
    /// </summary>
    public string Domain { get; init; } = string.Empty;

    /// <summary>
    /// Service name.
    /// </summary>
    public string ServiceName { get; init; } = string.Empty;

    /// <summary>
    /// SSL enabled.
    /// </summary>
    public bool SslEnabled { get; init; } = true;

    /// <summary>
    /// Container ID (optional).
    /// </summary>
    public int? ContainerId { get; init; }
}