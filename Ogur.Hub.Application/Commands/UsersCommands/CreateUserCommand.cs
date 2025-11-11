// File: Ogur.Hub.Application/Commands/UsersCommands/CreateUserCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.UsersCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Commands.UsersCommands;

/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="Username">Username.</param>
/// <param name="Email">Email address.</param>
/// <param name="Password">Password (will be hashed).</param>
/// <param name="IsActive">Whether user is active.</param>
/// <param name="IsAdmin">Whether user is admin.</param>
public sealed record CreateUserCommand(
    string Username,
    string Email,
    string Password,
    bool IsActive,
    bool IsAdmin);

/// <summary>
/// Result of creating a user.
/// </summary>
/// <param name="UserId">Created user ID.</param>
public sealed record CreateUserResult(int UserId);

/// <summary>
/// Handler for CreateUserCommand.
/// </summary>
public sealed class CreateUserCommandHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IRepository<User, int> _userRepository;

    /// <summary>
    /// Initializes a new instance of the CreateUserCommandHandler.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="userRepository">User repository.</param>
    public CreateUserCommandHandler(
        IApplicationDbContext context,
        IRepository<User, int> userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result with user ID.</returns>
    public async Task<Result<CreateUserResult>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        // Check if user with same username or email already exists
        var usernameExists = await _userRepository.AnyAsync(u => u.Username == command.Username, ct);
        if (usernameExists)
            return Result<CreateUserResult>.Failure($"User with username '{command.Username}' already exists");

        var emailExists = await _userRepository.AnyAsync(u => u.Email == command.Email, ct);
        if (emailExists)
            return Result<CreateUserResult>.Failure($"User with email '{command.Email}' already exists");

        // Hash password before creating user
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        // Create user (User.Create expects: username, email, passwordHash, isAdmin)
        var user = User.Create(
            command.Username,
            command.Email,
            passwordHash,
            command.IsAdmin);

        // Handle IsActive separately if user is created as active by default
        if (!command.IsActive)
        {
            user.Deactivate(); // Assuming you have this method
        }

        await _userRepository.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);

        var result = new CreateUserResult(user.Id);
        return Result<CreateUserResult>.Success(result);
    }
}