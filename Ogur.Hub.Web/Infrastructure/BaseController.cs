// File: Hub.Web/Infrastructure/BaseController.cs
// Project: Hub.Web
// Namespace: Ogur.Hub.Web.Infrastructure

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ogur.Hub.Web.Controllers;

namespace Ogur.Hub.Web.Infrastructure;

/// <summary>
/// Base controller providing session-based authentication and common functionality
/// </summary>
public abstract class BaseController : Controller
{
    /// <summary>
    /// Gets the authentication token from session
    /// </summary>
    protected string? AuthToken => HttpContext.Session.GetString("AuthToken");
    
    /// <summary>
    /// Gets the username from session
    /// </summary>
    protected string? Username => HttpContext.Session.GetString("Username");
    
    /// <summary>
    /// Gets the user ID from session
    /// </summary>
    protected int? UserId
    {
        get
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
    
    /// <summary>
    /// Gets whether the current user is an admin
    /// </summary>
    protected bool IsAdmin
    {
        get
        {
            var isAdminStr = HttpContext.Session.GetString("IsAdmin");
            return bool.TryParse(isAdminStr, out var isAdmin) && isAdmin;
        }
    }
    
    /// <summary>
    /// Checks if user is authenticated
    /// </summary>
    protected bool IsAuthenticated => !string.IsNullOrEmpty(AuthToken);

    /// <summary>
    /// Called before action execution to check authentication
    /// </summary>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!IsAuthenticated && context.Controller is not AccountController)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }
        
        base.OnActionExecuting(context);
    }
}