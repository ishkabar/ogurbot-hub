// File: Ogur.Hub.Web/Controllers/LicensesController.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing licenses.
/// </summary>
public class LicensesController : Controller
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<LicensesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LicensesController"/> class.
    /// </summary>
    public LicensesController(IHubApiClient hubApiClient, ILogger<LicensesController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays list of licenses.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int? applicationId)
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var licenses = await _hubApiClient.GetLicensesAsync(token, applicationId);
            
            if (licenses == null)
            {
                _logger.LogWarning("Failed to retrieve licenses - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Licenses";
            return View(licenses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving licenses");
            TempData["ErrorMessage"] = "Unable to load licenses. Please try again.";
            return View(new List<LicenseDto>());
        }
    }

    /// <summary>
    /// Displays license details.
    /// </summary>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewData["Title"] = "License Details";
        ViewBag.LicenseId = id;
        return View();
    }
}