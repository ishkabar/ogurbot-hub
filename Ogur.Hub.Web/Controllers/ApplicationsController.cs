// File: Hub.Web/Controllers/ApplicationsController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing applications
/// </summary>
public sealed class ApplicationsController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<ApplicationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the ApplicationsController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public ApplicationsController(IHubApiClient hubApiClient, ILogger<ApplicationsController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays list of applications
    /// </summary>
    /// <returns>Applications view</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var applications = await _hubApiClient.GetApplicationsAsync(AuthToken!);
            
            if (applications == null)
            {
                _logger.LogWarning("Failed to retrieve applications - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new ApplicationsViewModel
            {
                Title = "Applications",
                Description = "Manage registered applications",
                Username = Username,
                IsAdmin = IsAdmin,
                Applications = applications
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applications");
            TempData["ErrorMessage"] = "Unable to load applications. Please try again.";
            
            var viewModel = new ApplicationsViewModel
            {
                Title = "Applications",
                Description = "Manage registered applications",
                Username = Username,
                IsAdmin = IsAdmin,
                Applications = new List<ApplicationDto>()
            };
            
            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays application details
    /// </summary>
    /// <param name="id">Application ID</param>
    /// <returns>Application details view</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var applications = await _hubApiClient.GetApplicationsAsync(AuthToken!);
            var application = applications?.FirstOrDefault(a => a.Id == id);
            
            if (application == null)
            {
                return NotFound();
            }

            var viewModel = new ApplicationDetailsViewModel
            {
                Title = "Application Details",
                Description = $"Details for {application.DisplayName}",
                Username = Username,
                IsAdmin = IsAdmin,
                EntityId = id,
                ControllerName = "Applications",
                Application = new ApplicationViewDto
                {
                    Id = application.Id,
                    Name = application.Name,
                    DisplayName = application.DisplayName,
                    Description = application.Description,
                    CurrentVersion = application.CurrentVersion,
                    IsActive = application.IsActive,
                    CreatedAt = application.CreatedAt,
                    ApiKey = "***hidden***"
                },
                LicensesCount = 0,
                ActiveLicensesCount = 0,
                DevicesCount = 0,
                ConnectedDevicesCount = 0
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application details for id {Id}", id);
            return NotFound();
        }
    }

    /// <summary>
    /// Displays create application form
    /// </summary>
    /// <returns>Create application view</returns>
    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new ApplicationCreateViewModel
        {
            Title = "New Application",
            Description = "Register a new application",
            Username = Username,
            IsAdmin = IsAdmin
        };
    
        return View(viewModel);
    }

    /// <summary>
    /// Displays edit application form
    /// </summary>
    /// <param name="id">Application ID</param>
    /// <returns>Edit application view</returns>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var applications = await _hubApiClient.GetApplicationsAsync(AuthToken!);
            var application = applications?.FirstOrDefault(a => a.Id == id);
            
            if (application == null)
            {
                return NotFound();
            }

            var viewModel = new ApplicationEditViewModel
            {
                Title = "Edit Application",
                Description = $"Edit {application.DisplayName}",
                Username = Username,
                IsAdmin = IsAdmin,
                EntityId = id,
                ControllerName = "Applications",
                Application = new ApplicationViewDto
                {
                    Id = application.Id,
                    Name = application.Name,
                    DisplayName = application.DisplayName,
                    Description = application.Description,
                    CurrentVersion = application.CurrentVersion,
                    IsActive = application.IsActive,
                    CreatedAt = application.CreatedAt
                }
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving application for edit, id {Id}", id);
            return NotFound();
        }
    }
}