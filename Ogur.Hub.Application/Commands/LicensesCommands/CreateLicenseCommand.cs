// File: Ogur.Hub.Application/Commands/LicensesCommands/CreateLicenseCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Application.DTO;
using Ogur.Hub.Domain.Entities;
using MediatR;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to create a new license.
/// </summary>
/// <param name="ApplicationId">Application ID.</param>
/// <param name="UserId">User ID.</param>
/// <param name="MaxDevices">Maximum devices allowed.</param>
/// <param name="StartDate">License start date.</param>
/// <param name="EndDate">License end date.</param>
public sealed record CreateLicenseCommand(
    int ApplicationId,
    int UserId,
    int MaxDevices = 2,
    DateTime? StartDate = null,
    DateTime? EndDate = null) : IRequest<LicenseDto>;

/// <summary>
/// Handler for creating licenses.
/// </summary>
public sealed class CreateLicenseCommandHandler : IRequestHandler<CreateLicenseCommand, LicenseDto>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLicenseCommandHandler"/> class.
    /// </summary>
    /// <param name="licenseRepository">License repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public CreateLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository ?? throw new ArgumentNullException(nameof(licenseRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<LicenseDto> Handle(CreateLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = License.Create(
            applicationId: request.ApplicationId,
            userId: request.UserId,
            maxDevices: request.MaxDevices,
            startDate: request.StartDate,
            endDate: request.EndDate);

        await _licenseRepository.AddAsync(license, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new LicenseDto(
            Id: license.Id,
            LicenseKey: license.LicenseKey.Value,
            ApplicationId: license.ApplicationId,
            ApplicationName: string.Empty,
            UserId: license.UserId,
            MaxDevices: license.MaxDevices,
            RegisteredDevices: 0,
            Status: license.IsActive ? Domain.Enums.LicenseStatus.Active : Domain.Enums.LicenseStatus.Inactive,
            IssuedAt: license.StartDate,
            ExpiresAt: license.EndDate,
            RevokedAt: null,
            RevocationReason: null,
            LastValidatedAt: null,
            ValidationCount: 0);
    }
}