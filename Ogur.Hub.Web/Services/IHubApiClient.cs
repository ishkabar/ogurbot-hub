// File: Ogur.Hub.Web/Services/IHubApiClient.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Services

namespace Ogur.Hub.Web.Services;

/// <summary>
/// Client for communicating with Ogur Hub API.
/// </summary>
public interface IHubApiClient
{
    /// <summary>
    /// Authenticates user and returns JWT token.
    /// </summary>
    Task<LoginResponse?> LoginAsync(string username, string password);

    /// <summary>
    /// Gets all applications.
    /// </summary>
    Task<List<ApplicationDto>?> GetApplicationsAsync(string token);

    /// <summary>
    /// Gets all licenses.
    /// </summary>
    Task<List<LicenseDto>?> GetLicensesAsync(string token, int? applicationId = null);

    /// <summary>
    /// Gets all devices.
    /// </summary>
    Task<List<DeviceDto>?> GetDevicesAsync(string token, int? licenseId = null);

    /// <summary>
    /// Creates a new license.
    /// </summary>
    Task<LicenseDto?> CreateLicenseAsync(string token, CreateLicenseRequest request);
}

/// <summary>
/// Login response from API.
/// </summary>
public record LoginResponse(
    string AccessToken, 
    string TokenType, 
    int ExpiresIn, 
    int UserId, 
    string Username, 
    bool IsAdmin);

/// <summary>
/// Application DTO from API.
/// </summary>
public record ApplicationDto(
    int Id, 
    string Name, 
    string DisplayName, 
    string? Description, 
    string CurrentVersion, 
    bool IsActive, 
    DateTime CreatedAt);

/// <summary>
/// License DTO from API.
/// </summary>
public record LicenseDto(
    int Id, 
    string LicenseKey, 
    int ApplicationId, 
    string ApplicationName, 
    int UserId, 
    int MaxDevices, 
    int RegisteredDevices, 
    int Status,  // ← ZMIENIONE: z string na int (enum w API)
    DateTime IssuedAt, 
    DateTime? ExpiresAt,
    DateTime? RevokedAt,
    string? RevocationReason,
    DateTime? LastValidatedAt,
    int ValidationCount);

/// <summary>
/// Device DTO from API.
/// </summary>
public record DeviceDto(
    int Id, 
    int LicenseId, 
    string Hwid, 
    string DeviceGuid, 
    string DeviceName, 
    int Status,  // ← ZMIENIONE: z string na int (enum DeviceStatus w API)
    string? LastIpAddress, 
    DateTime RegisteredAt, 
    DateTime LastSeenAt,
    DateTime? BlockedAt,
    string? BlockReason,
    string? ConnectionId,
    DateTime? ConnectedAt);

/// <summary>
/// Request to create a new license.
/// </summary>
public record CreateLicenseRequest(
    int ApplicationId, 
    int UserId, 
    int MaxDevices, 
    DateTime? StartDate, 
    DateTime? EndDate);