// File: Hub.Application/Commands/LicensesCommands/ExtendLicenseCommand.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using MediatR;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to extend license expiration
/// </summary>
public sealed record ExtendLicenseCommand(int LicenseId, DateTime? NewExpirationDate) : IRequest<bool>;

/// <summary>
/// Handler for ExtendLicenseCommand
/// </summary>
public sealed class ExtendLicenseCommandHandler : IRequestHandler<ExtendLicenseCommand, bool>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes handler
    /// </summary>
    public ExtendLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the extend license command
    /// </summary>
    public async Task<bool> Handle(ExtendLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);

        if (license is null)
        {
            return false;
        }

        license.ExtendLicense(request.NewExpirationDate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}