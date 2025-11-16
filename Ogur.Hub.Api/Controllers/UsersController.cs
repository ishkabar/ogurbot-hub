// File: Ogur.Hub.Api/Controllers/UsersController.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Controllers

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ogur.Hub.Api.Models.Responses;
using Ogur.Hub.Application.Commands.UsersCommands;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Application.Queries.Users;
using Ogur.Hub.Api.Models.Requests;

namespace Ogur.Hub.Api.Controllers;

/// <summary>
/// Controller for user management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController : ControllerBase
{
    private readonly GetUsersQueryHandler _getUsersHandler;
    private readonly GetUserByIdQueryHandler _getUserByIdHandler;
    private readonly UpdateUserCommandHandler _updateUserHandler;
    private readonly CreateUserCommandHandler _createUserHandler;
    private readonly DeleteUserCommandHandler _deleteUserHandler;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Initializes a new instance of the UsersController.
    /// </summary>
    /// <param name="getUsersHandler">Get users query handler.</param>
    /// <param name="getUserByIdHandler">Get user by ID query handler.</param>
    /// <param name="updateUserHandler">Update user command handler.</param>
    /// <param name="createUserHandler">Create user command handler.</param>
    /// <param name="deleteUserHandler">Delete user command handler.</param>
    /// <param name="logger">Logger instance.</param>
    public UsersController(
        GetUsersQueryHandler getUsersHandler,
        GetUserByIdQueryHandler getUserByIdHandler,
        UpdateUserCommandHandler updateUserHandler,
        CreateUserCommandHandler createUserHandler,
        DeleteUserCommandHandler deleteUserHandler,
        ILogger<UsersController> logger)
    {
        _getUsersHandler = getUsersHandler;
        _getUserByIdHandler = getUserByIdHandler;
        _updateUserHandler = updateUserHandler;
        _createUserHandler = createUserHandler;
        _deleteUserHandler = deleteUserHandler;
        _logger = logger;
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        _logger.LogInformation("Getting all users");

        var result = await _getUsersHandler.Handle(new GetUsersQuery(), ct);

        return Ok(ApiResponse<IReadOnlyList<UserDto>>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">User creation request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created user details.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating new user {Username}", request.Username);

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(ApiResponse<UserDto>.ErrorResponse("Username and email are required"));
        }

        var command = new CreateUserCommand(
            request.Username,
            request.Email,
            request.Password,
            request.IsActive,
            request.Role);

        var result = await _createUserHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<UserDto>.ErrorResponse(result.Error!));
        }

        var dto = new UserDto(
            result.Value.UserId,
            request.Username,
            request.Email,
            request.IsActive,
            request.Role,
            request.Role == Domain.Enums.UserRole.Admin,
            0,
            DateTime.UtcNow,
            null);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.UserId },
            ApiResponse<UserDto>.SuccessResponse(dto));
    }

    /// <summary>
    /// Gets user by ID.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>User details.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        _logger.LogInformation("Getting user {UserId}", id);

        var result = await _getUserByIdHandler.Handle(new GetUserByIdQuery(id), ct);

        if (!result.IsSuccess)
        {
            return NotFound(ApiResponse<UserDto>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<UserDto>.SuccessResponse(result.Value));
    }

    /// <summary>
    /// Updates user details.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="request">Update request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success result.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Updating user {UserId}", id);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));
        }

        var command = new UpdateUserCommand(id, request.Email, request.IsActive, request.Role);
        var result = await _updateUserHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { message = "User updated successfully" }));
    }
    
    
    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id">User ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success result.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        _logger.LogInformation("Deleting user {UserId}", id);

        var command = new DeleteUserCommand(id);
        var result = await _deleteUserHandler.Handle(command, ct);

        if (!result.IsSuccess)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<object>.SuccessResponse(new { message = "User deleted successfully" }));
    }
}
