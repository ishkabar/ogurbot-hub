// File: Hub.Web/Services/IHubApiClient.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Services

using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Api.Models.Responses;


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
    
    /// <summary>
    /// Gets application by ID
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="id">Application ID</param>
    /// <returns>Application details</returns>
    Task<ApplicationDto?> GetApplicationByIdAsync(string token, int id);
    
    // W IHubApiClient.cs po GetApplicationByIdAsync:

    /// <summary>
    /// Updates an application
    /// </summary>
    /// <param name="token">Authentication token</param>
    /// <param name="id">Application ID</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated application</returns>
    Task<ApplicationDto?> UpdateApplicationAsync(string token, int id, UpdateApplicationRequest request);
    
    Task<RegisterResponse?> RegisterAsync(string username, string password, string? email = null);

}
