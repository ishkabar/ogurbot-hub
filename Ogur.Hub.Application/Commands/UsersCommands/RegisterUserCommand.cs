// File: Ogur.Hub.Application/Commands/UsersCommands/RegisterUserCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.UsersCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.Common.Results;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Domain.Enums;
using BC = BCrypt.Net.BCrypt;

namespace Ogur.Hub.Application.Commands.UsersCommands;

/// <summary>
/// Command to register a new user.
/// </summary>
/// <param name="Username">Username.</param>
/// <param name="Password">Password (will be hashed).</param>
/// <param name="Email">Email address (optional).</param>
public sealed record RegisterUserCommand(
    string Username,
    string Password,
    string? Email = null);

/// <summary>
/// Handler for RegisterUserCommand.
/// </summary>
public sealed class RegisterUserCommandHandler
{
    private readonly IRepository<User, int> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">User repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public RegisterUserCommandHandler(
        IRepository<User, int> userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Result with user ID if successful.</returns>
    public async Task<Result<int>> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        // Check if username already exists
        var existingUser = await _userRepository.FirstOrDefaultAsync(
            u => u.Username.ToLower() == command.Username.ToLower(),
            ct);

        if (existingUser != null)
        {
            return Result<int>.Failure("Username already exists");
        }

        // Check if email already exists (if provided)
        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            var existingEmail = await _userRepository.FirstOrDefaultAsync(
                u => u.Email.ToLower() == command.Email.ToLower(),
                ct);

            if (existingEmail != null)
            {
                return Result<int>.Failure("Email already exists");
            }
        }

        // Hash password
        var passwordHash = BC.HashPassword(command.Password);

        // Use email or generate a placeholder email if not provided
        var email = string.IsNullOrWhiteSpace(command.Email)
            ? $"{command.Username.ToLower()}@noemail.local"
            : command.Email;

        // Create user with default User role (flag = 1)
        var user = User.Create(
            username: command.Username,
            email: email,
            passwordHash: passwordHash,
            role: UserRole.User);

        await _userRepository.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<int>.Success(user.Id);
    }
}
