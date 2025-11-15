namespace Ogur.Hub.Api.Models.Requests;

/// <summary>
/// Request model for updating user.
/// </summary>
/// <param name="Email">Email address.</param>
/// <param name="IsActive">Is user active.</param>
/// <param name="IsAdmin">Is user admin.</param>
public record UpdateUserRequest(string Email, bool IsActive, bool IsAdmin);