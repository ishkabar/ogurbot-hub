// File: Hub.Application/Commands/LicensesCommands/UpdateLicenseCommand.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using MediatR;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Domain.Entities;
using Ogur.Hub.Domain.Enums;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to update a license
/// </summary>
public sealed record UpdateLicenseCommand(
    int LicenseId,
    int MaxDevices,
    DateTime? ExpiresAt,
    int Status) : IRequest<LicenseDto?>;

/// <summary>
/// Handler for UpdateLicenseCommand
/// </summary>
public sealed class UpdateLicenseCommandHandler : IRequestHandler<UpdateLicenseCommand, LicenseDto?>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes handler
    /// </summary>
    public UpdateLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the update license command
    /// </summary>
    public async Task<LicenseDto?> Handle(UpdateLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);

        if (license is null)
        {
            return null;
        }

        license.Update(request.MaxDevices, request.ExpiresAt, (LicenseStatus)request.Status);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LicenseDto(
            license.Id,
            license.LicenseKey.Value,
            license.ApplicationId,
            license.Application?.DisplayName ?? string.Empty,
            license.UserId,
            license.MaxDevices,
            license.Devices.Count,
            license.Status,
            license.StartDate,
            license.EndDate,
            null,
            null,
            null,
            0);
    }
}