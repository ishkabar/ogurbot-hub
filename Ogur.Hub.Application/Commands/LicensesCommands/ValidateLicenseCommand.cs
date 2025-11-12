// File: Ogur.Hub.Application/Commands/LicensesCommands/ValidateLicenseCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to validate a license and register/update device.
/// </summary>
/// <param name="LicenseKey">License key to validate.</param>
/// <param name="ApplicationId">Application ID.</param>
/// <param name="Hwid">Hardware ID.</param>
/// <param name="DeviceGuid">Device GUID.</param>
/// <param name="DeviceName">Device name.</param>
public sealed record ValidateLicenseCommand(
    string LicenseKey,
    int ApplicationId,
    string Hwid,
    Guid DeviceGuid,
    string? DeviceName = null) : IRequest<LicenseValidationResult>;

/// <summary>
/// Result of license validation.
/// </summary>
public sealed record LicenseValidationResult
{
    /// <summary>
    /// Whether the license is valid.
    /// </summary>
    public required bool IsValid { get; init; }

    /// <summary>
    /// Device identifier if registered.
    /// </summary>
    public int? DeviceId { get; init; }

    /// <summary>
    /// Whether this is a new device registration.
    /// </summary>
    public bool IsNewDevice { get; init; }

    /// <summary>
    /// License expiration date.
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// Number of devices registered.
    /// </summary>
    public int RegisteredDevices { get; init; }

    /// <summary>
    /// Maximum devices allowed.
    /// </summary>
    public int MaxDevices { get; init; }

    /// <summary>
    /// Error message if validation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// License details if valid.
    /// </summary>
    public LicenseDto? License { get; init; }
}

/// <summary>
/// Handler for validating licenses and registering devices.
/// </summary>
public sealed class ValidateLicenseCommandHandler : IRequestHandler<ValidateLicenseCommand, LicenseValidationResult>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IRepository<Device, int> _deviceRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateLicenseCommandHandler"/> class.
    /// </summary>
    /// <param name="licenseRepository">License repository.</param>
    /// <param name="deviceRepository">Device repository.</param>
    /// <param name="context">Database context.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public ValidateLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IRepository<Device, int> deviceRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository ?? throw new ArgumentNullException(nameof(licenseRepository));
        _deviceRepository = deviceRepository ?? throw new ArgumentNullException(nameof(deviceRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<LicenseValidationResult> Handle(ValidateLicenseCommand request, CancellationToken cancellationToken)
    {
        var licenseKey = LicenseKey.Create(request.LicenseKey);
        
        var license = await _context.Licenses
            .Include(l => l.Devices)
            .FirstOrDefaultAsync(l => l.LicenseKey == licenseKey && l.ApplicationId == request.ApplicationId, cancellationToken);

        if (license == null)
        {
            return new LicenseValidationResult
            {
                IsValid = false,
                ErrorMessage = "Invalid license key",
                RegisteredDevices = 0,
                MaxDevices = 0
            };
        }

        if (!license.IsValid())
        {
            return new LicenseValidationResult
            {
                IsValid = false,
                ErrorMessage = "License is not active or has expired",
                RegisteredDevices = license.Devices.Count,
                MaxDevices = license.MaxDevices,
                ExpiresAt = license.EndDate
            };
        }

        var fingerprint = DeviceFingerprint.Create(request.Hwid, request.DeviceGuid);
        
        var existingDevice = await _deviceRepository.FirstOrDefaultAsync(
            d => d.LicenseId == license.Id && d.Fingerprint == fingerprint,
            cancellationToken);

        if (existingDevice != null)
        {
            existingDevice.UpdateLastSeen(null);
            _deviceRepository.Update(existingDevice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var licenseDto = CreateLicenseDto(license);
            return new LicenseValidationResult
            {
                IsValid = true,
                DeviceId = existingDevice.Id,
                IsNewDevice = false,
                ExpiresAt = license.EndDate,
                RegisteredDevices = license.Devices.Count,
                MaxDevices = license.MaxDevices,
                License = licenseDto
            };
        }

        if (!license.CanRegisterDevice())
        {
            return new LicenseValidationResult
            {
                IsValid = false,
                ErrorMessage = "Maximum number of devices reached for this license",
                RegisteredDevices = license.Devices.Count,
                MaxDevices = license.MaxDevices,
                ExpiresAt = license.EndDate
            };
        }

        var newDevice = Device.Create(license.Id, fingerprint, request.DeviceName);
        await _deviceRepository.AddAsync(newDevice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = CreateLicenseDto(license);
        return new LicenseValidationResult
        {
            IsValid = true,
            DeviceId = newDevice.Id,
            IsNewDevice = true,
            ExpiresAt = license.EndDate,
            RegisteredDevices = license.Devices.Count + 1,
            MaxDevices = license.MaxDevices,
            License = dto
        };
    }

    private static LicenseDto CreateLicenseDto(License license)
    {
        return new LicenseDto(
            Id: license.Id,
            LicenseKey: license.LicenseKey.Value,
            ApplicationId: license.ApplicationId,
            ApplicationName: string.Empty,
            UserId: license.UserId,
            MaxDevices: license.MaxDevices,
            RegisteredDevices: license.Devices.Count,
            Status: license.IsActive ? Domain.Enums.LicenseStatus.Active : Domain.Enums.LicenseStatus.Inactive,
            IssuedAt: license.StartDate,
            ExpiresAt: license.EndDate,
            RevokedAt: null,
            RevocationReason: null,
            LastValidatedAt: DateTime.UtcNow,
            ValidationCount: 0,
            Description:license.Description);
    }
}