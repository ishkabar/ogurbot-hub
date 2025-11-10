// File: Hub.Web/Controllers/HomeController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;
using System.Diagnostics;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for home and dashboard pages
/// </summary>
public sealed class HomeController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public HomeController(IHubApiClient hubApiClient, ILogger<HomeController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays dashboard
    /// </summary>
    /// <returns>Dashboard view with statistics</returns>
    public async Task<IActionResult> Index()
    {
        try
        {
            var stats = await _hubApiClient.GetDashboardStatsAsync(AuthToken!);
            
            var viewModel = new DashboardViewModel
            {
                Title = "Dashboard",
                Description = "Welcome to Ogur.Hub Admin Panel",
                Username = Username,
                IsAdmin = IsAdmin,
                Stats = stats ?? new DashboardStatsDto()
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard");
            
            var viewModel = new DashboardViewModel
            {
                Title = "Dashboard",
                Description = "Welcome to Ogur.Hub Admin Panel",
                Username = Username,
                IsAdmin = IsAdmin,
                Stats = new DashboardStatsDto()
            };
            
            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays privacy policy
    /// </summary>
    /// <returns>Privacy view</returns>
    public IActionResult Privacy()
    {
        return View();
    }
    
// File: Hub.Web/Controllers/HomeController.cs
// W środku klasy HomeController dodaj:

    /// <summary>
    /// Displays settings page
    /// </summary>
    /// <returns>Settings view</returns>
    public IActionResult Settings()
    {
        var viewModel = new SettingsViewModel
        {
            Title = "Settings",
            Description = "Manage your account settings",
            Username = Username,
            IsAdmin = IsAdmin,
            Theme = "light",
            Language = "en"
        };

        return View(viewModel);
    }

    /// <summary>
    /// Displays error page
    /// </summary>
    /// <returns>Error view</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}