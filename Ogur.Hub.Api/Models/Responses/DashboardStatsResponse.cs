// File: Hub.Api/Models/Responses/DashboardStatsResponse.cs
// Project: Hub.Api
// Namespace: Ogur.Hub.Api.Models.Responses

namespace Ogur.Hub.Api.Models.Responses;

/// <summary>
/// Dashboard statistics response.
/// </summary>
public sealed record DashboardStatsResponse
{
    /// <summary>
    /// Total number of applications.
    /// </summary>
    public required int TotalApplications { get; init; }

    /// <summary>
    /// Number of active licenses.
    /// </summary>
    public required int ActiveLicenses { get; init; }

    /// <summary>
    /// Number of currently connected devices.
    /// </summary>
    public required int ConnectedDevices { get; init; }

    /// <summary>
    /// Number of commands sent today.
    /// </summary>
    public required int CommandsToday { get; init; }

    /// <summary>
    /// Total number of users.
    /// </summary>
    public required int TotalUsers { get; init; }

    /// <summary>
    /// Total number of devices.
    /// </summary>
    public required int TotalDevices { get; init; }

    /// <summary>
    /// Number of expired licenses.
    /// </summary>
    public required int ExpiredLicenses { get; init; }

    /// <summary>
    /// Number of revoked licenses.
    /// </summary>
    public required int RevokedLicenses { get; init; }
}