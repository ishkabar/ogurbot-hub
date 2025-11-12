// File: Ogur.Hub.Application/DTO/LicenseDto.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.DTO

using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Application.DTO;

/// <summary>
/// DTO representing a license.
/// </summary>
/// <param name="Id">License ID.</param>
/// <param name="LicenseKey">License key.</param>
/// <param name="ApplicationId">Application ID.</param>
/// <param name="ApplicationName">Application name.</param>
/// <param name="UserId">User ID.</param>
/// <param name="MaxDevices">Maximum devices allowed.</param>
/// <param name="RegisteredDevices">Number of registered devices.</param>
/// <param name="Status">License status.</param>
/// <param name="IssuedAt">Issue timestamp.</param>
/// <param name="ExpiresAt">Expiration timestamp.</param>
/// <param name="RevokedAt">Revocation timestamp.</param>
/// <param name="RevocationReason">Revocation reason.</param>
/// <param name="LastValidatedAt">Last validation timestamp.</param>
/// <param name="ValidationCount">Validation count.</param>
/// /// <param name="Description">License description</param>

public sealed record LicenseDto(
    int Id,
    string LicenseKey,
    int ApplicationId,
    string ApplicationName,
    int UserId,
    int MaxDevices,
    int RegisteredDevices,
    LicenseStatus Status,
    DateTime IssuedAt,
    DateTime? ExpiresAt,
    DateTime? RevokedAt,
    string? RevocationReason,
    DateTime? LastValidatedAt,
    int ValidationCount,
    string? Description);
