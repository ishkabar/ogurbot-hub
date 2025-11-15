// File: Hub.Web/Controllers/UsersController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Api.Models.Requests;


namespace Ogur.Hub.Web.Controllers;

/// <summary>
/// Controller for managing users
/// </summary>
public sealed class UsersController : BaseController
{
    private readonly IHubApiClient _hubApiClient;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Initializes a new instance of the UsersController
    /// </summary>
    /// <param name="hubApiClient">Hub API client for backend communication</param>
    /// <param name="logger">Logger instance</param>
    public UsersController(IHubApiClient hubApiClient, ILogger<UsersController> logger)
    {
        _hubApiClient = hubApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Displays list of users
    /// </summary>
    /// <returns>Users view</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var users = await _hubApiClient.GetUsersAsync(AuthToken!);
            
            if (users == null)
            {
                _logger.LogWarning("Failed to retrieve users - token may be expired");
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new UsersViewModel
            {
                Title = "Users",
                Description = "Manage user accounts and permissions",
                Username = Username,
                IsAdmin = IsAdmin,
                Users = users
            };
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            TempData["ErrorMessage"] = "Unable to load users. Please try again.";
            
            var viewModel = new UsersViewModel
            {
                Title = "Users",
                Description = "Manage user accounts and permissions",
                Username = Username,
                IsAdmin = IsAdmin,
                Users = new List<UserDto>()
            };
            
            return View(viewModel);
        }
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request">User creation request</param>
    /// <returns>JSON result</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!IsAdmin)
            {
                return Json(new { success = false, message = "Only administrators can create users" });
            }

            var user = await _hubApiClient.CreateUserAsync(AuthToken!, request);
            
            if (user == null)
            {
                return Json(new { success = false, message = "Failed to create user" });
            }

            _logger.LogInformation("User {Username} created by admin {AdminUsername}", 
                request.Username, Username);

            return Json(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {Username}", request.Username);
            return Json(new { success = false, message = "An error occurred while creating the user" });
        }
    }

    /// <summary>
    /// Displays user details
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details view</returns>
    [HttpGet]
    public IActionResult Details(int id)
    {
        var viewModel = new UserDetailsViewModel
        {
            Title = "User Details",
            Description = "View user details and license information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Users",
            EntityId = id,
            UserId = id
        };
        
        return View(viewModel);
    }

    /// <summary>
    /// Displays user edit page
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User edit view</returns>
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var viewModel = new UserEditViewModel
        {
            Title = "Edit User",
            Description = "Update user information and permissions",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Users",
            EntityId = id,
            UserId = id
        };
        
        return View(viewModel);
    }
}