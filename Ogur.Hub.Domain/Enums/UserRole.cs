// File: Ogur.Hub.Domain/Enums/UserRole.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Enums

namespace Ogur.Hub.Domain.Enums;

/// <summary>
/// Represents user role levels in the system using flags for combined permissions.
/// </summary>
[Flags]
public enum UserRole
{
    /// <summary>
    /// No permissions.
    /// </summary>
    None = 0,

    /// <summary>
    /// Can use applications (login to apps without Hub access).
    /// </summary>
    User = 1,

    /// <summary>
    /// Can view Hub (read-only access to Hub panel).
    /// </summary>
    Viewer = 2,

    /// <summary>
    /// Can modify Hub data (create/edit licenses, devices, etc).
    /// </summary>
    Moderator = 4,

    /// <summary>
    /// Full administrative access to all features.
    /// </summary>
    Admin = 8,

    /// <summary>
    /// User + Viewer (can use apps and view Hub).
    /// </summary>
    UserWithViewer = User | Viewer,

    /// <summary>
    /// User + Moderator (can use apps and modify Hub).
    /// </summary>
    UserWithModerator = User | Moderator
}