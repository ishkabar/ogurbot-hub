namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for creating user.
/// </summary>
/// <param name="Username">Username.</param>
/// <param name="Email">Email address.</param>
/// <param name="Password">Password.</param>
/// <param name="IsActive">Is user active.</param>
/// <param name="IsAdmin">Is user admin.</param>
public record CreateUserRequest(
    string Username, 
    string Email, 
    string Password, 
    bool IsActive, 
    bool IsAdmin);