// File: Hub.Web/Controllers/VpsController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;
using Ogur.Hub.Application.DTO;



namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for VPS monitoring
/// </summary>
public sealed class VpsController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<VpsController> _logger;
    
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };


    /// <summary>
    /// Initializes a new instance of the VpsController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="configuration">Configuration instance</param>
    /// <param name="logger">Logger instance</param>
    public VpsController(
        IHubApiClient hubApiClient,
        IConfiguration configuration,
        ILogger<VpsController> logger)
    {
        _hubApiClient = hubApiClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Displays VPS monitoring page
    /// </summary>
    /// <returns>VPS monitoring view</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var containers = await _hubApiClient.GetContainersAsync(AuthToken!);
            var websites = await _hubApiClient.GetWebsitesAsync(AuthToken!);
            var currentResources = await _hubApiClient.GetCurrentResourcesAsync(AuthToken!);

            var apiBaseUrl = _configuration["HubApi:BaseUrl"]
                             ?? throw new InvalidOperationException("HubApi:BaseUrl not configured");

            var viewModel = new VpsMonitoringViewModel
            {
                Title = "VPS Monitoring",
                Description = "Monitor VPS resources, containers, and websites",
                Username = Username,
                IsAdmin = IsAdmin,
                CurrentResources = currentResources,
                Containers = containers ?? new List<VpsContainerDto>(),
                Websites = websites ?? new List<VpsWebsiteDto>(),
                ApiBaseUrl = apiBaseUrl
            };

            ViewBag.JwtToken = AuthToken;
            ViewBag.VpsDataJson = JsonSerializer.Serialize(new
            {
                Containers = containers,
                Websites = websites,
                CurrentResources = currentResources,
                ApiBaseUrl = _configuration["ApiSettings:BaseUrl"]
            }, _jsonOptions);
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load VPS monitoring data");
            TempData["ErrorMessage"] = "Unable to load VPS monitoring data. Please try again.";

            var apiBaseUrl = _configuration["HubApi:BaseUrl"] ?? "http://localhost:5180";

            var viewModel = new VpsMonitoringViewModel
            {
                Title = "VPS Monitoring",
                Description = "Monitor VPS resources, containers, and websites",
                Username = Username,
                IsAdmin = IsAdmin,
                CurrentResources = null,
                Containers = new List<VpsContainerDto>(),
                Websites = new List<VpsWebsiteDto>(),
                ApiBaseUrl = apiBaseUrl
            };

            ViewBag.JwtToken = AuthToken;

            return View(viewModel);
        }
    }

    /// <summary>
    /// Refreshes Docker containers data
    /// </summary>
    /// <returns>JSON result indicating success</returns>
    [HttpPost]
    public async Task<IActionResult> RefreshContainers()
    {
        try
        {
            var success = await _hubApiClient.RefreshContainersAsync(AuthToken!);
            return Json(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing containers");
            return Json(new { success = false, message = "Error refreshing containers" });
        }
    }

    /// <summary>
    /// Refreshes websites health check data
    /// </summary>
    /// <returns>JSON result indicating success</returns>
    [HttpPost]
    public async Task<IActionResult> RefreshWebsites()
    {
        try
        {
            var success = await _hubApiClient.RefreshWebsitesAsync(AuthToken!);
            return Json(new { success });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing websites");
            return Json(new { success = false, message = "Error refreshing websites" });
        }
    }
}