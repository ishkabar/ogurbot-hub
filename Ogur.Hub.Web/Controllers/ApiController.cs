// File: Hub.Web/Controllers/ApiController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// API controller for providing data to frontend components
/// </summary>
[Route("api/[action]")]
public sealed class ApiController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<ApiController> _logger;

    /// <summary>
    /// Initializes a new instance of the ApiController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public ApiController(IHubApiClient hubApiClient, ILogger<ApiController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Gets all applications for dropdown/select components
    /// </summary>
    /// <returns>JSON list of applications</returns>
    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        try
        {
            var applications = await _hubApiClient.GetApplicationsAsync(AuthToken!);
            
            if (applications == null)
            {
                return Json(new { success = false, message = "Failed to load applications" });
            }

            return Json(new { success = true, data = applications });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications for API");
            return Json(new { success = false, message = "An error occurred while loading applications" });
        }
    }

    /// <summary>
    /// Gets all users for dropdown/select components
    /// </summary>
    /// <returns>JSON list of users</returns>
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _hubApiClient.GetUsersAsync(AuthToken!);
            
            if (users == null)
            {
                return Json(new { success = false, message = "Failed to load users" });
            }

            return Json(new { success = true, data = users });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for API");
            return Json(new { success = false, message = "An error occurred while loading users" });
        }
    }

    /// <summary>
    /// Gets all licenses for dropdown/select components
    /// </summary>
    /// <param name="applicationId">Optional filter by application ID</param>
    /// <returns>JSON list of licenses</returns>
    [HttpGet]
    public async Task<IActionResult> GetLicenses(int? applicationId = null)
    {
        try
        {
            var licenses = await _hubApiClient.GetLicensesAsync(AuthToken!, applicationId);
            
            if (licenses == null)
            {
                return Json(new { success = false, message = "Failed to load licenses" });
            }

            return Json(new { success = true, data = licenses });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving licenses for API");
            return Json(new { success = false, message = "An error occurred while loading licenses" });
        }
    }

    /// <summary>
    /// Gets all devices for dropdown/select components
    /// </summary>
    /// <param name="licenseId">Optional filter by license ID</param>
    /// <returns>JSON list of devices</returns>
    [HttpGet]
    public async Task<IActionResult> GetDevices(int? licenseId = null)
    {
        try
        {
            var devices = await _hubApiClient.GetDevicesAsync(AuthToken!, licenseId);
            
            if (devices == null)
            {
                return Json(new { success = false, message = "Failed to load devices" });
            }

            return Json(new { success = true, data = devices });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices for API");
            return Json(new { success = false, message = "An error occurred while loading devices" });
        }
    }
}