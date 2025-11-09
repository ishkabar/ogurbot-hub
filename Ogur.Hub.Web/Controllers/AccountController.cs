// File: Ogur.Hub.Web/Controllers/AccountController.cs
// Project: Ogur.Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Models;
using Ogur.Hub.Web.Services;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for account management (login, logout).
/// </summary>
public class AccountController : Controller
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    public AccountController(IHubApiClient hubApiClient, ILogger<AccountController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays login page.
    /// </summary>
    [HttpGet]
    public IActionResult Login()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (!string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["Title"] = "Sign In";
        return View();
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        _logger.LogInformation("=== LOGIN POST CALLED ===");
        _logger.LogInformation("Username received: '{Username}'", model?.Username ?? "NULL");
        _logger.LogInformation("Password length: {Length}", model?.Password?.Length ?? 0);
        _logger.LogInformation("ModelState valid: {IsValid}", ModelState.IsValid);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _hubApiClient.LoginAsync(model.Username, model.Password);

            if (response == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            HttpContext.Session.SetString("AuthToken", response.AccessToken);
            HttpContext.Session.SetString("Username", response.Username);
            HttpContext.Session.SetString("UserId", response.UserId.ToString());
            HttpContext.Session.SetString("IsAdmin", response.IsAdmin.ToString());

            _logger.LogInformation("User {Username} logged in successfully", response.Username);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "Unable to connect to authentication server. Please try again later.");
            return View(model);
        }
    }

    /// <summary>
    /// Logs out current user.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var username = HttpContext.Session.GetString("Username");
        HttpContext.Session.Clear();
        
        _logger.LogInformation("User {Username} logged out", username);
        
        return RedirectToAction("Login");
    }
}