// File: Ogur.Hub.Api/Controllers/AuthController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ogur.Hub.Application.Commands.UsersCommands;
using Ogur.Hub.Api.Models.Requests;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Api.Services;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using BC = BCrypt.Net.BCrypt;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for authentication operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IRepository<User, int> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    /// <param name="tokenService">Token service.</param>
    /// <param name="logger">Logger instance.</param>
    public AuthController(
        IRepository<User, int> userRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Login response with JWT token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var user = users.FirstOrDefault(u => 
            u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) ||
            u.Email.Equals(request.Username, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            _logger.LogWarning("Login attempt with non-existent username: {Username}", request.Username);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Invalid username or password"));
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt for inactive user: {Username}", user.Username);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("User account is inactive"));
        }

        if (user.IsLockedOut())
        {
            _logger.LogWarning("Login attempt for locked out user: {Username}", user.Username);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("User account is locked out"));
        }

        if (!BC.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Failed login attempt for user: {Username}", user.Username);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse("Invalid username or password"));
        }

        user.RecordSuccessfulLogin();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.IsAdmin);
        var expirationMinutes = int.Parse(HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["Jwt:ExpirationMinutes"] ?? "1440");

        var response = new LoginResponse
        {
            AccessToken = token,
            ExpiresIn = expirationMinutes * 60,
            UserId = user.Id,
            Username = user.Username,
            IsAdmin = user.IsAdmin,
            Role = (int)user.Role,
        };

        _logger.LogInformation("User {Username} logged in successfully", user.Username);

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(response));
    }
    
    
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">Registration request.</param>
    /// <param name="handler">Register user command handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Registration response.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        [FromServices] RegisterUserCommandHandler handler,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<RegisterResponse>.ErrorResponse("Invalid registration data"));
        }

        var command = new RegisterUserCommand(
            Username: request.Username,
            Password: request.Password,
            Email: request.Email);

        var result = await handler.Handle(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Registration failed for username {Username}: {Error}", request.Username, result.Error);
            return BadRequest(ApiResponse<RegisterResponse>.ErrorResponse(result.Error!));
        }

        _logger.LogInformation("New user registered: {Username} (ID: {UserId})", request.Username, result.Value);

        var response = new RegisterResponse
        {
            UserId = result.Value,
            Username = request.Username,
            Message = "Registration successful! You can now login."
        };

        return Created($"/api/users/{result.Value}", ApiResponse<RegisterResponse>.SuccessResponse(response));
    }

}