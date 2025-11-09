// File: Ogur.Hub.Web/Controllers/HomeController.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Models;
using System.Diagnostics;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for home and dashboard pages.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays dashboard.
    /// </summary>
    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var username = HttpContext.Session.GetString("Username");
        ViewData["Title"] = "Dashboard";
        ViewBag.Username = username;
        return View();
    }

    /// <summary>
    /// Displays privacy policy.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays error page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }
}