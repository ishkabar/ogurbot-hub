// File: Ogur.Hub.Web/Controllers/DevicesController.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing devices.
/// </summary>
public class DevicesController : Controller
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<DevicesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DevicesController"/> class.
    /// </summary>
    public DevicesController(IHubApiClient hubApiClient, ILogger<DevicesController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays list of devices.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int? licenseId)
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var devices = await _hubApiClient.GetDevicesAsync(token, licenseId);
            
            if (devices == null)
            {
                _logger.LogWarning("Failed to retrieve devices - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Devices";
            return View(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving devices");
            TempData["ErrorMessage"] = "Unable to load devices. Please try again.";
            return View(new List<DeviceDto>());
        }
    }

    /// <summary>
    /// Displays device details.
    /// </summary>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewData["Title"] = "Device Details";
        ViewBag.DeviceId = id;
        return View();
    }
}