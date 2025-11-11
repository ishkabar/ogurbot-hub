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
                Description = "Manage and monitor applications",
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
                Description = "Manage and monitor applications",
                Username = Username,
                IsAdmin = IsAdmin,
                Applications = new List<ApplicationDto>()
            };
            
            return View(viewModel);
        }
    }

    /// <summary>
    /// Creates a new application
    /// </summary>
    /// <param name="request">Application creation request</param>
    /// <returns>JSON result</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request)
    {
        try
        {
            if (!IsAdmin)
            {
                return Json(new { success = false, message = "Only administrators can create applications" });
            }

            var application = await _hubApiClient.CreateApplicationAsync(AuthToken!, request);
            
            if (application == null)
            {
                return Json(new { success = false, message = "Failed to create application" });
            }

            _logger.LogInformation("Application {Name} created by user {Username}", 
                request.Name, Username);

            return Json(new { success = true, data = application });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application {Name}", request.Name);
            return Json(new { success = false, message = "An error occurred while creating the application" });
        }
    }

    /// <summary>
    /// Displays application details
    /// </summary>
    /// <param name="id">Application ID</param>
    /// <returns>Application details view</returns>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var viewModel = new ApplicationDetailsViewModel
        {
            Title = "Application Details",
            Description = "View application details and statistics",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Applications",
            EntityId = id,
            ApplicationId = id
        };
        
        return View(viewModel);
    }

    /// <summary>
    /// Displays application edit page
    /// </summary>
    /// <param name="id">Application ID</param>
    /// <returns>Application edit view</returns>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var viewModel = new ApplicationEditViewModel
        {
            Title = "Edit Application",
            Description = "Update application information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Applications",
            EntityId = id,
            ApplicationId = id
        };
        
        return View(viewModel);
    }
}