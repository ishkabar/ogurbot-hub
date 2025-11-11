// File: Hub.Application/Commands/LicensesCommands/ActivateLicenseCommand.cs
// Project: Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using MediatR;
using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to activate a license
/// </summary>
public sealed record ActivateLicenseCommand(int LicenseId) : IRequest<bool>;

/// <summary>
/// Handler for ActivateLicenseCommand
/// </summary>
public sealed class ActivateLicenseCommandHandler : IRequestHandler<ActivateLicenseCommand, bool>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes handler
    /// </summary>
    public ActivateLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the activate license command
    /// </summary>
    public async Task<bool> Handle(ActivateLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);

        if (license is null)
        {
            return false;
        }

        license.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}