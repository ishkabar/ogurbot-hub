// File: Ogur.Hub.Application/DTO/DashboardStatsDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// Dashboard statistics data transfer object.
/// </summary>
public sealed class DashboardStatsDto
{
    /// <summary>
    /// Gets or sets the total number of applications.
    /// </summary>
    public int TotalApplications { get; init; }
    
    /// <summary>
    /// Gets or sets the number of active applications.
    /// </summary>
    public int ActiveApplications { get; init; }
    
    /// <summary>
    /// Gets or sets the total number of licenses.
    /// </summary>
    public int TotalLicenses { get; init; }
    
    /// <summary>
    /// Gets or sets the number of active licenses.
    /// </summary>
    public int ActiveLicenses { get; init; }
    
    /// <summary>
    /// Gets or sets the number of expired licenses.
    /// </summary>
    public int ExpiredLicenses { get; init; }
    
    /// <summary>
    /// Gets or sets the number of revoked licenses.
    /// </summary>
    public int RevokedLicenses { get; init; }
    
    /// <summary>
    /// Gets or sets the total number of devices.
    /// </summary>
    public int TotalDevices { get; init; }
    
    /// <summary>
    /// Gets or sets the number of online devices.
    /// </summary>
    public int OnlineDevices { get; init; }
    
    /// <summary>
    /// Gets or sets the number of currently connected devices.
    /// </summary>
    public int ConnectedDevices { get; init; }
    
    /// <summary>
    /// Gets or sets the total number of users.
    /// </summary>
    public int TotalUsers { get; init; }
    
    /// <summary>
    /// Gets or sets the number of commands sent today.
    /// </summary>
    public int CommandsToday { get; init; }
}