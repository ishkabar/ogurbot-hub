// File: Ogur.Hub.Api/Models/Responses/ApiResponse.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Responses

namespace Ogur.Hub.Api.Models.Responses;

/// <summary>
/// Generic API response wrapper.
/// </summary>
/// <typeparam name="T">Type of the response data.</typeparam>
public sealed record ApiResponse<T>
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Response data.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Error message if the request failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Timestamp of the response.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="data">Response data.</param>
    /// <returns>Successful API response.</returns>
    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    /// <summary>
    /// Creates an error response.
    /// </summary>
    /// <param name="error">Error message.</param>
    /// <returns>Error API response.</returns>
    public static ApiResponse<T> ErrorResponse(string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error
        };
    }
}