// File: Ogur.Hub.Api/Models/Responses/ErrorResponse.cs
// Project: Ogur.Hub.Api
// Namespace: Ogur.Hub.Api.Models.Responses

namespace Ogur.Hub.Api.Models.Responses;

/// <summary>
/// Error response model.
/// </summary>
public sealed record ErrorResponse
{
    /// <summary>
    /// Error message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Error code.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Additional error details.
    /// </summary>
    public Dictionary<string, string[]>? Details { get; init; }

    /// <summary>
    /// Timestamp of the error.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}