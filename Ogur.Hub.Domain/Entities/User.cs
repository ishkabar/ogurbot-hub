// File: Ogur.Hub.Domain/Entities/User.cs
// Project: Ogur.Hub.Domain
// Namespace: Ogur.Hub.Domain.Entities

using Ogur.Hub.Domain.Common;
using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Domain.Entities;

/// <summary>
/// Represents a user account for the web panel.
/// </summary>
public sealed class User : AggregateRoot<int>
{
    /// <summary>
    /// Gets the username.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// Gets the email address.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the password hash (BCrypt or similar).
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// Gets the user role.
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Gets whether the user is an administrator (backward compatibility helper).
    /// </summary>
    public bool IsAdmin => Role.HasFlag(UserRole.Admin);

    /// <summary>
    /// Gets whether the account is active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the last login timestamp.
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Gets the failed login attempt count.
    /// </summary>
    public int FailedLoginAttempts { get; private set; }

    /// <summary>
    /// Gets when the account was locked out.
    /// </summary>
    public DateTime? LockedOutUntil { get; private set; }

    private User() { }

    private User(string username, string email, string passwordHash, UserRole role)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        FailedLoginAttempts = 0;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="username">Username.</param>
    /// <param name="email">Email address.</param>
    /// <param name="passwordHash">Hashed password.</param>
    /// <param name="role">User role.</param>
    /// <returns>A new User instance.</returns>
    public static User Create(string username, string email, string passwordHash, UserRole role = UserRole.User)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));

        return new User(username, email, passwordHash, role);
    }

    /// <summary>
    /// Creates a new user with legacy IsAdmin parameter (backward compatibility).
    /// </summary>
    /// <param name="username">Username.</param>
    /// <param name="email">Email address.</param>
    /// <param name="passwordHash">Hashed password.</param>
    /// <param name="isAdmin">Whether user is admin.</param>
    /// <returns>A new User instance.</returns>
    public static User Create(string username, string email, string passwordHash, bool isAdmin)
    {
        var role = isAdmin ? UserRole.Admin : UserRole.User;
        return Create(username, email, passwordHash, role);
    }

    /// <summary>
    /// Records a successful login.
    /// </summary>
    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedOutUntil = null;
    }

    /// <summary>
    /// Records a failed login attempt.
    /// </summary>
    /// <param name="lockoutThreshold">Number of failed attempts before lockout (default: 5).</param>
    /// <param name="lockoutDurationMinutes">Lockout duration in minutes (default: 15).</param>
    public void RecordFailedLogin(int lockoutThreshold = 5, int lockoutDurationMinutes = 15)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= lockoutThreshold)
        {
            LockedOutUntil = DateTime.UtcNow.AddMinutes(lockoutDurationMinutes);
        }
    }

    /// <summary>
    /// Checks if the account is currently locked out.
    /// </summary>
    /// <returns>True if locked out, false otherwise.</returns>
    public bool IsLockedOut()
    {
        if (!LockedOutUntil.HasValue)
            return false;

        if (LockedOutUntil.Value <= DateTime.UtcNow)
        {
            LockedOutUntil = null;
            FailedLoginAttempts = 0;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Updates the password.
    /// </summary>
    /// <param name="newPasswordHash">New password hash.</param>
    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    /// <summary>
    /// Updates user details.
    /// </summary>
    /// <param name="email">New email address.</param>
    public void Update(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        Email = email;
    }

    /// <summary>
    /// Sets the user role.
    /// </summary>
    /// <param name="role">New role.</param>
    public void SetRole(UserRole role)
    {
        Role = role;
        UpdateTimestamp();
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Checks if user has at least the specified role level.
    /// </summary>
    /// <param name="requiredRole">Minimum required role.</param>
    /// <returns>True if user has required role or higher.</returns>
    public bool HasRole(UserRole requiredRole)
    {
        return Role.HasFlag(requiredRole);
    }

    /// <summary>
    /// Checks if user can access Hub (Viewer, Moderator, or Admin).
    /// </summary>
    /// <returns>True if user has Hub access.</returns>
    public bool CanAccessHub()
    {
        return Role.HasFlag(UserRole.Viewer) || 
               Role.HasFlag(UserRole.Moderator) || 
               Role.HasFlag(UserRole.Admin);
    }

    /// <summary>
    /// Checks if user can modify Hub data (Moderator or Admin).
    /// </summary>
    /// <returns>True if user can modify Hub.</returns>
    public bool CanModifyHub()
    {
        return Role.HasFlag(UserRole.Moderator) || Role.HasFlag(UserRole.Admin);
    }
}
