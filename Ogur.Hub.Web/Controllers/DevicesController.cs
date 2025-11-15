// File: Hub.Web/Controllers/DevicesController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;
using Ogur.Hub.Application.DTO;


namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing devices
/// </summary>
public sealed class DevicesController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DevicesController> _logger;

    /// <summary>
    /// Initializes a new instance of the DevicesController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="configuration">Configuration instance</param>
    /// <param name="logger">Logger instance</param>
    public DevicesController(IHubApiClient hubApiClient, IConfiguration configuration, ILogger<DevicesController> logger)
    {
        _hubApiClient = hubApiClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Displays list of devices
    /// </summary>
    /// <param name="licenseId">Optional filter by license ID</param>
    /// <returns>Devices view</returns>
    [HttpGet]
    public async Task<IActionResult> Index(int? licenseId)
    {
        try
        {
            var devices = await _hubApiClient.GetDevicesAsync(AuthToken!, licenseId);
            
            if (devices == null)
            {
                _logger.LogWarning("Failed to retrieve devices - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new DevicesViewModel
            {
                Title = "Devices",
                Description = "Monitor and manage connected devices",
                Username = Username,
                IsAdmin = IsAdmin,
                Devices = devices,
                LicenseId = licenseId
            };
            
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices");
            TempData["ErrorMessage"] = "Unable to load devices. Please try again.";
            
            var viewModel = new DevicesViewModel
            {
                Title = "Devices",
                Description = "Monitor and manage connected devices",
                Username = Username,
                IsAdmin = IsAdmin,
                Devices = new List<DeviceDto>(),
                LicenseId = licenseId
            };
            
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays device details
    /// </summary>
    /// <param name="id">Device ID</param>
    /// <returns>Device details view</returns>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var viewModel = new DeviceDetailsViewModel
        {
            Title = "Device Details",
            Description = "View device details and session information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Devices",
            EntityId = id,
            DeviceId = id
        };
        
        return View(viewModel);
    }

    /// <summary>
    /// Displays device edit page
    /// </summary>
    /// <param name="id">Device ID</param>
    /// <returns>Device edit view</returns>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var viewModel = new DeviceEditViewModel
        {
            Title = "Edit Device",
            Description = "Update device information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Devices",
            EntityId = id,
            DeviceId = id
        };
        
        return View(viewModel);
    }
}