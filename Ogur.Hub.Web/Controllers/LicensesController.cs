// File: Hub.Web/Controllers/LicensesController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing licenses
/// </summary>
public sealed class LicensesController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<LicensesController> _logger;

    /// <summary>
    /// Initializes a new instance of the LicensesController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public LicensesController(IHubApiClient hubApiClient, ILogger<LicensesController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }
    
    /// <summary>
    /// Displays create license form
    /// </summary>
    /// <returns>Create license view</returns>
    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new LicenseCreateViewModel
        {
            Title = "New License",
            Description = "Register a new license",
            Username = Username,
            IsAdmin = IsAdmin
        };
    
        return View(viewModel);
    }

    /// <summary>
    /// Displays list of licenses
    /// </summary>
    /// <param name="applicationId">Optional filter by application ID</param>
    /// <returns>Licenses view</returns>
    [HttpGet]
    public async Task<IActionResult> Index(int? applicationId)
    {
        try
        {
            var licenses = await _hubApiClient.GetLicensesAsync(AuthToken!, applicationId);
            
            if (licenses == null)
            {
                _logger.LogWarning("Failed to retrieve licenses - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new LicensesViewModel
            {
                Title = "Licenses",
                Description = "Monitor and manage application licenses",
                Username = Username,
                IsAdmin = IsAdmin,
                Licenses = licenses,
                ApplicationId = applicationId
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving licenses");
            TempData["ErrorMessage"] = "Unable to load licenses. Please try again.";
            
            var viewModel = new LicensesViewModel
            {
                Title = "Licenses",
                Description = "Monitor and manage application licenses",
                Username = Username,
                IsAdmin = IsAdmin,
                Licenses = new List<LicenseDto>(),
                ApplicationId = applicationId
            };
            
            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays license details
    /// </summary>
    /// <param name="id">License ID</param>
    /// <returns>License details view</returns>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var viewModel = new LicenseDetailsViewModel
        {
            Title = "License Details",
            Description = "View license details and device information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Licenses",
            EntityId = id,
            LicenseId = id
        };
        
        return View(viewModel);
    }

    /// <summary>
    /// Displays license edit page
    /// </summary>
    /// <param name="id">License ID</param>
    /// <returns>License edit view</returns>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var viewModel = new LicenseEditViewModel
        {
            Title = "Edit License",
            Description = "Update license information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Licenses",
            EntityId = id,
            LicenseId = id
        };
        
        return View(viewModel);
    }
}