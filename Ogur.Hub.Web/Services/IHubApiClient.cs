// File: Hub.Web/Services/IHubApiClient.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Services

using Ogur.Hub.Web.Models.ViewModels;

namespace Ogur.Hub.Web.Services;

/// <summary>
/// Client for communicating with Ogur Hub API
/// </summary>
public interface IHubApiClient
{
    /// <summary>
    /// Authenticates user and returns JWT token
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>Login response with access token</returns>
    Task<LoginResponse?> LoginAsync(string username, string password);

    /// <summary>
    /// Gets dashboard statistics
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>Dashboard statistics</returns>
    Task<DashboardStatsDto?> GetDashboardStatsAsync(string token);

    /// <summary>
    /// Gets all applications
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>List of applications</returns>
    Task<List<ApplicationDto>?> GetApplicationsAsync(string token);

    /// <summary>
    /// Gets all licenses
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="applicationId">Optional filter by application ID</param>
    /// <returns>List of licenses</returns>
    Task<List<LicenseDto>?> GetLicensesAsync(string token, int? applicationId = null);

    /// <summary>
    /// Gets all devices
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="licenseId">Optional filter by license ID</param>
    /// <returns>List of devices</returns>
    Task<List<DeviceDto>?> GetDevicesAsync(string token, int? licenseId = null);

    /// <summary>
    /// Gets all users
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>List of users</returns>
    Task<List<UserDto>?> GetUsersAsync(string token);

    /// <summary>
    /// Creates a new license
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="request">License creation request</param>
    /// <returns>Created license</returns>
    Task<LicenseDto?> CreateLicenseAsync(string token, CreateLicenseRequest request);

    /// <summary>
    /// Creates a new application
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="request">Application creation request</param>
    /// <returns>Created application</returns>
    Task<ApplicationDto?> CreateApplicationAsync(string token, CreateApplicationRequest request);

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="request">User creation request</param>
    /// <returns>Created user</returns>
    Task<UserDto?> CreateUserAsync(string token, CreateUserRequest request);

    /// <summary>
    /// Creates a new device manually (optional - normally devices are auto-registered via license validation)
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="request">Device creation request</param>
    /// <returns>Created device</returns>
    Task<DeviceDto?> CreateDeviceAsync(string token, CreateDeviceRequest request);

    /// <summary>
    /// Gets all VPS containers
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>List of containers</returns>
    Task<List<VpsContainerDto>?> GetContainersAsync(string token);

    /// <summary>
    /// Gets all monitored websites
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>List of websites</returns>
    Task<List<VpsWebsiteDto>?> GetWebsitesAsync(string token);

    /// <summary>
    /// Gets current VPS resources
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>Current resource usage</returns>
    Task<VpsResourceDto?> GetCurrentResourcesAsync(string token);

    /// <summary>
    /// Gets VPS resource history
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="from">Start date</param>
    /// <param name="to">End date</param>
    /// <returns>List of resource snapshots</returns>
    Task<List<VpsResourceDto>?> GetResourceHistoryAsync(string token, DateTime from, DateTime to);

    /// <summary>
    /// Refreshes container statistics
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>True if successful</returns>
    Task<bool> RefreshContainersAsync(string token);

    /// <summary>
    /// Refreshes website health checks
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <returns>True if successful</returns>
    Task<bool> RefreshWebsitesAsync(string token);
}

#region Authentication DTOs

/// <summary>
/// Login response containing access token and user information
/// </summary>
/// <param name="AccessToken">JWT access token</param>
/// <param name="TokenType">Token type (Bearer)</param>
/// <param name="ExpiresIn">Token expiration in seconds</param>
/// <param name="UserId">User ID</param>
/// <param name="Username">Username</param>
/// <param name="IsAdmin">Whether user is admin</param>
public record LoginResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    int UserId,
    string Username,
    bool IsAdmin);

#endregion

#region Application DTOs

/// <summary>
/// Application data transfer object
/// </summary>
/// <param name="Id">Application ID</param>
/// <param name="Name">Application name</param>
/// <param name="DisplayName">Display name</param>
/// <param name="Description">Description</param>
/// <param name="CurrentVersion">Current version</param>
/// <param name="IsActive">Whether application is active</param>
/// <param name="CreatedAt">Creation timestamp</param>
public record ApplicationDto(
    int Id,
    string Name,
    string DisplayName,
    string? Description,
    string CurrentVersion,
    bool IsActive,
    DateTime CreatedAt);

/// <summary>
/// Create application request
/// </summary>
/// <param name="Name">Application name</param>
/// <param name="DisplayName">Display name</param>
/// <param name="Description">Optional description</param>
/// <param name="CurrentVersion">Current version</param>
/// <param name="IsActive">Whether application is active</param>
public record CreateApplicationRequest(
    string Name,
    string DisplayName,
    string? Description,
    string CurrentVersion,
    bool IsActive);

#endregion

#region License DTOs

/// <summary>
/// License data transfer object
/// </summary>
/// <param name="Id">License ID</param>
/// <param name="LicenseKey">License key</param>
/// <param name="ApplicationId">Application ID</param>
/// <param name="ApplicationName">Application name</param>
/// <param name="UserId">User ID</param>
/// <param name="MaxDevices">Maximum allowed devices</param>
/// <param name="RegisteredDevices">Number of registered devices</param>
/// <param name="Status">License status</param>
/// <param name="IssuedAt">Issue timestamp</param>
/// <param name="ExpiresAt">Expiration timestamp</param>
/// <param name="RevokedAt">Revocation timestamp</param>
/// <param name="RevocationReason">Revocation reason</param>
/// <param name="LastValidatedAt">Last validation timestamp</param>
/// <param name="ValidationCount">Number of validations</param>
public record LicenseDto(
    int Id,
    string LicenseKey,
    int ApplicationId,
    string ApplicationName,
    int UserId,
    int MaxDevices,
    int RegisteredDevices,
    int Status,
    DateTime IssuedAt,
    DateTime? ExpiresAt,
    DateTime? RevokedAt,
    string? RevocationReason,
    DateTime? LastValidatedAt,
    int ValidationCount);

/// <summary>
/// Create license request
/// </summary>
/// <param name="ApplicationId">Application ID</param>
/// <param name="UserId">User ID</param>
/// <param name="MaxDevices">Maximum allowed devices</param>
/// <param name="StartDate">Optional start date</param>
/// <param name="EndDate">Optional end date</param>
public record CreateLicenseRequest(
    int ApplicationId,
    int UserId,
    int MaxDevices,
    DateTime? StartDate,
    DateTime? EndDate);

#endregion

#region Device DTOs

/// <summary>
/// Device data transfer object
/// </summary>
/// <param name="Id">Device ID</param>
/// <param name="LicenseId">License ID</param>
/// <param name="Hwid">Hardware ID</param>
/// <param name="DeviceGuid">Device GUID</param>
/// <param name="DeviceName">Device name</param>
/// <param name="Status">Device status</param>
/// <param name="LastIpAddress">Last IP address</param>
/// <param name="RegisteredAt">Registration timestamp</param>
/// <param name="LastSeenAt">Last seen timestamp</param>
/// <param name="BlockedAt">Block timestamp</param>
/// <param name="BlockReason">Block reason</param>
/// <param name="ConnectionId">SignalR connection ID</param>
/// <param name="ConnectedAt">Connection timestamp</param>
public record DeviceDto(
    int Id,
    int LicenseId,
    string Hwid,
    string DeviceGuid,
    string DeviceName,
    int Status,
    string? LastIpAddress,
    DateTime RegisteredAt,
    DateTime LastSeenAt,
    DateTime? BlockedAt,
    string? BlockReason,
    string? ConnectionId,
    DateTime? ConnectedAt);

/// <summary>
/// Create device request (if manually adding devices)
/// </summary>
/// <param name="LicenseId">License ID</param>
/// <param name="Hwid">Hardware ID</param>
/// <param name="DeviceGuid">Device GUID</param>
/// <param name="DeviceName">Device name</param>
public record CreateDeviceRequest(
    int LicenseId,
    string Hwid,
    string DeviceGuid,
    string DeviceName);

#endregion

#region User DTOs

/// <summary>
/// User data transfer object for list view
/// </summary>
/// <param name="Id">User ID</param>
/// <param name="Username">Username</param>
/// <param name="Email">Email address</param>
/// <param name="IsActive">Is user active</param>
/// <param name="IsAdmin">Is user admin</param>
/// <param name="LicensesCount">Number of licenses</param>
/// <param name="CreatedAt">Created timestamp</param>
/// <param name="LastLoginAt">Last login timestamp</param>
public record UserDto(
    int Id,
    string Username,
    string Email,
    bool IsActive,
    bool IsAdmin,
    int LicensesCount,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

/// <summary>
/// Create user request
/// </summary>
/// <param name="Username">Username</param>
/// <param name="Email">Email address</param>
/// <param name="Password">Password</param>
/// <param name="IsActive">Whether user is active</param>
/// <param name="IsAdmin">Whether user is admin</param>
public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    bool IsActive,
    bool IsAdmin);

#endregion

#region VPS DTOs

/// <summary>
/// VPS container data transfer object
/// </summary>
/// <param name="Id">Container ID</param>
/// <param name="ContainerId">Docker container ID</param>
/// <param name="Name">Container name</param>
/// <param name="Image">Docker image</param>
/// <param name="State">Container state</param>
/// <param name="Status">Container status</param>
/// <param name="CreatedAt">Creation timestamp</param>
/// <param name="StartedAt">Start timestamp</param>
/// <param name="CpuUsagePercent">CPU usage percentage</param>
/// <param name="MemoryUsageMb">Memory usage in MB</param>
/// <param name="MemoryLimitMb">Memory limit in MB</param>
/// <param name="MemoryUsagePercent">Memory usage percentage</param>
/// <param name="NetworkRxMb">Network received in MB</param>
/// <param name="NetworkTxMb">Network transmitted in MB</param>
/// <param name="LastUpdatedAt">Last update timestamp</param>
public record VpsContainerDto(
    int Id,
    string ContainerId,
    string Name,
    string Image,
    string State,
    string Status,
    DateTime CreatedAt,
    DateTime? StartedAt,
    decimal CpuUsagePercent,
    decimal MemoryUsageMb,
    decimal MemoryLimitMb,
    decimal MemoryUsagePercent,
    decimal NetworkRxMb,
    decimal NetworkTxMb,
    DateTime LastUpdatedAt);

/// <summary>
/// VPS website data transfer object
/// </summary>
/// <param name="Id">Website ID</param>
/// <param name="Domain">Domain name</param>
/// <param name="ServiceName">Service name</param>
/// <param name="ContainerName">Container name</param>
/// <param name="SslEnabled">Whether SSL is enabled</param>
/// <param name="SslExpiresAt">SSL expiration timestamp</param>
/// <param name="IsActive">Whether website is active</param>
/// <param name="LastCheckedAt">Last check timestamp</param>
/// <param name="LastStatusCode">Last HTTP status code</param>
/// <param name="LastResponseTimeMs">Last response time in milliseconds</param>
/// <param name="SslExpiringSoon">Whether SSL is expiring soon</param>
public record VpsWebsiteDto(
    int Id,
    string Domain,
    string ServiceName,
    string? ContainerName,
    bool SslEnabled,
    DateTime? SslExpiresAt,
    bool IsActive,
    DateTime LastCheckedAt,
    int? LastStatusCode,
    int? LastResponseTimeMs,
    bool SslExpiringSoon);


/// <summary>
/// Update application request
/// </summary>
/// <param name="Name">Application name</param>
/// <param name="DisplayName">Display name</param>
/// <param name="Description">Description</param>
/// <param name="CurrentVersion">Current version</param>
/// <param name="IsActive">Whether application is active</param>
public record UpdateApplicationRequest(
    string Name,
    string DisplayName,
    string? Description,
    string CurrentVersion,
    bool IsActive);

/// <summary>
/// Update license request
/// </summary>
/// <param name="MaxDevices">Maximum allowed devices</param>
/// <param name="ExpiresAt">Expiration date</param>
/// <param name="Status">License status</param>
public record UpdateLicenseRequest(
    int MaxDevices,
    DateTime? ExpiresAt,
    int Status);

/// <summary>
/// Update user request
/// </summary>
/// <param name="Username">Username</param>
/// <param name="Email">Email address</param>
/// <param name="IsActive">Whether user is active</param>
/// <param name="IsAdmin">Whether user is admin</param>
/// <param name="Password">New password (optional)</param>
public record UpdateUserRequest(
    string Username,
    string Email,
    bool IsActive,
    bool IsAdmin,
    string? Password);

/// <summary>
/// VPS resource data transfer object
/// </summary>
/// <param name="CpuUsagePercent">CPU usage percentage</param>
/// <param name="MemoryTotalGb">Total memory in GB</param>
/// <param name="MemoryUsedGb">Used memory in GB</param>
/// <param name="MemoryUsagePercent">Memory usage percentage</param>
/// <param name="DiskTotalGb">Total disk in GB</param>
/// <param name="DiskUsedGb">Used disk in GB</param>
/// <param name="DiskUsagePercent">Disk usage percentage</param>
/// <param name="NetworkRxMbps">Network received in Mbps</param>
/// <param name="NetworkTxMbps">Network transmitted in Mbps</param>
/// <param name="LoadAverage1Min">1-minute load average</param>
/// <param name="LoadAverage5Min">5-minute load average</param>
/// <param name="LoadAverage15Min">15-minute load average</param>
/// <param name="Timestamp">Snapshot timestamp</param>
public record VpsResourceDto(
    decimal CpuUsagePercent,
    decimal MemoryTotalGb,
    decimal MemoryUsedGb,
    decimal MemoryUsagePercent,
    decimal DiskTotalGb,
    decimal DiskUsedGb,
    decimal DiskUsagePercent,
    decimal NetworkRxMbps,
    decimal NetworkTxMbps,
    decimal LoadAverage1Min,
    decimal LoadAverage5Min,
    decimal LoadAverage15Min,
    DateTime Timestamp);

#endregion