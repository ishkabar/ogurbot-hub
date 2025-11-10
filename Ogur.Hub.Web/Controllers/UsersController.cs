// File: Hub.Web/Controllers/UsersController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Controllers

using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Web.Infrastructure;
using Ogur.Hub.Web.Models.ViewModels;
using Ogur.Hub.Web.Services;

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
            
            // If API returns 404 (not implemented), show empty list with info message
            if (users == null)
            {
                _logger.LogInformation("Users API endpoint not yet implemented or returned null");
                users = new List<UserDto>();
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
            Description = "View user details and associated licenses",
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
            Description = "Update user information",
            Username = Username,
            IsAdmin = IsAdmin,
            ControllerName = "Users",
            EntityId = id,
            UserId = id
        };
        
        return View(viewModel);
    }
    
    /// <summary>
    /// Displays create user form
    /// </summary>
    /// <returns>Create user view</returns>
    [HttpGet]
    public IActionResult Create()
    {
        var viewModel = new UserCreateViewModel
        {
            Title = "New User",
            Description = "Register a new user",
            Username = Username,
            IsAdmin = IsAdmin
        };
    
        return View(viewModel);
    }
}