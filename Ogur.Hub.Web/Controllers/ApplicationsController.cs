// File: Hub.Web/Controllers/ApplicationsController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Domain.Enums;


namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing applications
/// </summary>
public sealed class ApplicationsController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApplicationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the ApplicationsController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public ApplicationsController(IHubApiClient hubApiClient, IConfiguration configuration,
        ILogger<ApplicationsController> logger)
    {
        _hubApiClient = hubApiClient;
        _configuration = configuration;
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
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
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

            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
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

    [HttpPost]
    public async Task<IActionResult> UpdateApplication([FromBody] UpdateApplicationDto dto)
    {
        var request = new UpdateApplicationRequest
        {
            Name = dto.Data.Name,
            DisplayName = dto.Data.DisplayName,
            Description = dto.Data.Description,
            CurrentVersion = dto.Data.CurrentVersion,
            IsActive = dto.Data.IsActive
        };

        var result = await _hubApiClient.UpdateApplicationAsync(AuthToken!, dto.Id, request);

        return Ok(new { success = result != null });
    }

    public record UpdateApplicationDto(int Id, ApplicationUpdateData Data);

    public record ApplicationUpdateData(
        string Name,
        string DisplayName,
        string? Description,
        string CurrentVersion,
        bool IsActive);

    /// <summary>
    /// Displays application details
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var application = await _hubApiClient.GetApplicationByIdAsync(AuthToken!, id);

        if (application == null)
        {
            return NotFound();
        }

        var licenses = await _hubApiClient.GetLicensesAsync(AuthToken!, id);

        var viewModel = new ApplicationDetailsViewModel
        {
            Title = "Application Details",
            Description = "Details for " + application.DisplayName,
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Applications",
            EntityId = id,
            ApplicationId = id,
            Application = application,
            LicensesCount = licenses?.Count ?? 0,
            ActiveLicensesCount = licenses?.Count(l => l.Status == LicenseStatus.Active) ?? 0,
            DevicesCount = licenses?.Sum(l => l.RegisteredDevices) ?? 0,
            ConnectedDevicesCount = 0
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return PartialView(viewModel);
        }

        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View(viewModel);
    }

    /// <summary>
    /// Displays application edit page
    /// </summary>
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

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            return PartialView(viewModel);
        }

        ViewBag.ApiBaseUrl = _configuration["ApiSettings:BaseUrl"];
        return View(viewModel);
    }
}