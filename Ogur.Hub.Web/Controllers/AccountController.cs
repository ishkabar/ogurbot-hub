// File: Hub.Web/Controllers/AccountController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Models;
using Ogur.Hub.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Ogur.Hub.Application.Commands.UsersCommands;

namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for account management
/// </summary>
public sealed class AccountController : Controller
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    const int HubAccessFlags = 2 | 4 | 8; // Viewer | Moderator | Admin

    /// <summary>
    /// Initializes a new instance of the AccountController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Configuration</param>
    /// <param name="environment">Environment</param>
    public AccountController(
        IHubApiClient hubApiClient,
        ILogger<AccountController> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Displays login page
    /// </summary>
    /// <returns>Login view</returns>
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var token = HttpContext.Session.GetString("AuthToken");
        if (!string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        // Auto-login in Development
        /*
        if (_environment.IsDevelopment())
        {
            var autoLoginEnabled = _configuration.GetValue<bool>("Development:AutoLogin:Enabled");
            if (autoLoginEnabled)
            {
                var username = _configuration["Development:AutoLogin:Username"];
                var password = _configuration["Development:AutoLogin:Password"];

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    _logger.LogInformation("Auto-login enabled for development - logging in as {Username}", username);

                    try
                    {
                        var response = await _hubApiClient.LoginAsync(username, password);

                        if (response != null)
                        {
                            HttpContext.Session.SetString("AuthToken", response.AccessToken);
                            HttpContext.Session.SetString("Username", response.Username);
                            HttpContext.Session.SetString("UserId", response.UserId.ToString());
                            HttpContext.Session.SetString("IsAdmin", response.IsAdmin.ToString());

                            _logger.LogInformation("Auto-login successful for {Username}", response.Username);
                            return RedirectToAction("Index", "Home");
                        }

                        _logger.LogWarning("Auto-login failed - invalid credentials");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Auto-login failed with exception");
                    }
                }
            }
        }
*/
        ViewData["Title"] = "Sign In";
        return View();
    }

    /// <summary>
    /// Handles login form submission
    /// </summary>
    /// <param name="model">Login view model</param>
    /// <returns>Redirect to dashboard or login view with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        _logger.LogInformation("Login attempt for username: {Username}", model?.Username ?? "NULL");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _hubApiClient.LoginAsync(model.Username, model.Password);

            if (response == null)
            {
                _logger.LogWarning("Login failed for username: {Username}", model.Username);
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            if ((response.Role & HubAccessFlags) == 0)
            {
                _logger.LogWarning("User {Username} (Role={Role}) attempted to access Hub without permissions",
                    response.Username, response.Role);

                // Build current role description
                var roleNames = new List<string>();
                if ((response.Role & 1) != 0) roleNames.Add("User");
                if ((response.Role & 2) != 0) roleNames.Add("Viewer");
                if ((response.Role & 4) != 0) roleNames.Add("Moderator");
                if ((response.Role & 8) != 0) roleNames.Add("Admin");

                string currentRole = roleNames.Any()
                    ? string.Join(", ", roleNames)
                    : "No permissions";

                ModelState.AddModelError(string.Empty,
                    $"Access denied. Your role: {currentRole}. Required: Viewer, Moderator, or Admin.");

                return View(model);
            }

            HttpContext.Session.SetString("AuthToken", response.AccessToken);
            HttpContext.Session.SetString("Username", response.Username);
            HttpContext.Session.SetString("UserId", response.UserId.ToString());
            HttpContext.Session.SetString("IsAdmin", response.IsAdmin.ToString());
            HttpContext.Session.SetString("Role", response.Role.ToString());

            _logger.LogInformation("User {Username} logged in successfully", response.Username);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty,
                "Unable to connect to authentication server. Please try again later.");
            return View(model);
        }
    }

    /// <summary>
    /// Displays the registration page.
    /// </summary>
    /// <returns>Register view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    /// <summary>
    /// Handles registration form submission.
    /// </summary>
    /// <param name="username">Username.</param>
    /// <param name="email">Email (optional).</param>
    /// <param name="password">Password.</param>
    /// <returns>JSON result.</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        try
        {
            var response = await _hubApiClient.RegisterAsync(username, password, email);

            if (response == null || response.UserId == 0)
            {
                var errorMessage = response?.Message ?? "Registration failed. Username or email may already exist.";
                _logger.LogWarning("Registration failed for username: {Username} - {Error}", username, errorMessage);
                return Json(new { success = false, error = errorMessage });
            }

            _logger.LogInformation("User {Username} registered successfully", username);
            return Json(new
                { success = true, message = response.Message ?? "Registration successful! You can now login." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for user {Username}", username);
            return Json(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Logs out current user
    /// </summary>
    /// <returns>Redirect to login page</returns>
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