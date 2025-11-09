// File: Ogur.Hub.Domain/Enums/LicenseStatus.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Enums

namespace Ogur.Hub.Domain.Enums;

/// <summary>
/// Represents the current status of a license.
/// </summary>
public enum LicenseStatus
{
    /// <summary>
    /// License is active and valid.
    /// </summary>
    Active = 1,

    /// <summary>
    /// License has expired.
    /// </summary>
    Expired = 2,

    /// <summary>
    /// License has been revoked by administrator.
    /// </summary>
    Revoked = 3,

    /// <summary>
    /// License is pending activation.
    /// </summary>
    Pending = 4,

    /// <summary>
    /// License is suspended temporarily.
    /// </summary>
    Suspended = 5,

    /// <summary>
    /// License is inactive.
    /// </summary>
    Inactive = 6
}