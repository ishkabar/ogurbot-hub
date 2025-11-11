// File: Hub.Web/Controllers/LicensesController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    private readonly IConfiguration _configuration;
    private readonly ILogger<LicensesController> _logger;

    /// <summary>
    /// Initializes a new instance of the LicensesController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public LicensesController(IHubApiClient hubApiClient, IConfiguration configuration, ILogger<LicensesController> logger)
    {
        _hubApiClient = hubApiClient;
        _configuration = configuration;
        _logger = logger;
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
            
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"]; 
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
    /// Creates a new license
    /// </summary>
    /// <param name="request">License creation request</param>
    /// <returns>JSON result</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLicenseRequest request)
    {
        try
        {
            if (!IsAdmin)
            {
                return Json(new { success = false, message = "Only administrators can create licenses" });
            }

            var license = await _hubApiClient.CreateLicenseAsync(AuthToken!, request);
            
            if (license == null)
            {
                return Json(new { success = false, message = "Failed to create license" });
            }

            _logger.LogInformation("License created for application {ApplicationId} and user {UserId} by admin {AdminUsername}", 
                request.ApplicationId, request.UserId, Username);

            return Json(new { success = true, data = license });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating license for application {ApplicationId} and user {UserId}", 
                request.ApplicationId, request.UserId);
            return Json(new { success = false, message = "An error occurred while creating the license" });
        }
    }
    
    /// <summary>
    /// Displays create license form
    /// </summary>
    /// <returns>Create license view</returns>
    [HttpGet]
    public IActionResult CreateView()
    {
        var viewModel = new LicenseCreateViewModel
        {
            Title = "New License",
            Description = "Register a new license",
            Username = Username,
            IsAdmin = IsAdmin
        };
    
        return View("Create", viewModel);
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