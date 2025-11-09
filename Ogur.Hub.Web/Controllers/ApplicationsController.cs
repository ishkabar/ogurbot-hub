// File: Ogur.Hub.Web/Controllers/ApplicationsController.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing applications.
/// </summary>
public class ApplicationsController : Controller
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<ApplicationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationsController"/> class.
    /// </summary>
    public ApplicationsController(IHubApiClient hubApiClient, ILogger<ApplicationsController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays list of applications.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var applications = await _hubApiClient.GetApplicationsAsync(token);
            
            if (applications == null)
            {
                _logger.LogWarning("Failed to retrieve applications - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = "Applications";
            return View(applications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications");
            TempData["ErrorMessage"] = "Unable to load applications. Please try again.";
            return View(new List<ApplicationDto>());
        }
    }

    /// <summary>
    /// Displays application details.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewData["Title"] = "Application Details";
        ViewBag.ApplicationId = id;
        return View();
    }

    /// <summary>
    /// Displays create application form.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewData["Title"] = "New Application";
        return View();
    }

    /// <summary>
    /// Displays edit application form.
    /// </summary>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewData["Title"] = "Edit Application";
        ViewBag.ApplicationId = id;
        return View();
    }
}