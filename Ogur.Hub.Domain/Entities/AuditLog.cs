// File: Ogur.Hub.Domain/Entities/AuditLog.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents an audit log entry for tracking all operations in the system.
/// </summary>
public sealed class AuditLog : Entity<int>
{
    /// <summary>
    /// Gets the user ID who performed the action (null for system actions).
    /// </summary>
    public int? UserId { get; private set; }

    /// <summary>
    /// Gets the action performed (e.g., "LicenseCreated", "DeviceBlocked").
    /// </summary>
    public string Action { get; private set; }

    /// <summary>
    /// Gets the entity type affected (e.g., "License", "Device").
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// Gets the entity ID affected.
    /// </summary>
    public int? EntityId { get; private set; }

    /// <summary>
    /// Gets additional details as JSON string.
    /// </summary>
    public string? DetailsJson { get; private set; }

    /// <summary>
    /// Gets the IP address of the request.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Gets when the action occurred.
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Gets the navigation property to the user.
    /// </summary>
    public User? User { get; private set; }

    private AuditLog() { }

    private AuditLog(
        int? userId,
        string action,
        string entityType,
        int? entityId,
        string? detailsJson,
        string? ipAddress)
    {
        UserId = userId;
        Action = action;
        EntityType = entityType;
        EntityId = entityId;
        DetailsJson = detailsJson;
        IpAddress = ipAddress;
        OccurredAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new audit log entry.
    /// </summary>
    /// <param name="userId">User ID (null for system actions).</param>
    /// <param name="action">Action performed.</param>
    /// <param name="entityType">Entity type affected.</param>
    /// <param name="entityId">Entity ID affected.</param>
    /// <param name="detailsJson">Additional details as JSON.</param>
    /// <param name="ipAddress">IP address.</param>
    /// <returns>A new AuditLog instance.</returns>
    public static AuditLog Create(
        int? userId,
        string action,
        string entityType,
        int? entityId = null,
        string? detailsJson = null,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action is required", nameof(action));

        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type is required", nameof(entityType));

        return new AuditLog(userId, action, entityType, entityId, detailsJson, ipAddress);
    }
}