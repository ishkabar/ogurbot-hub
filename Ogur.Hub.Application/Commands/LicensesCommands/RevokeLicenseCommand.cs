// File: Ogur.Hub.Application/Commands/LicensesCommands/RevokeLicenseCommand.cs
// Project: Ogur.Hub.Application
// Namespace: Ogur.Hub.Application.Commands.LicensesCommands

using Ogur.Hub.Application.Common.Interfaces;
using Ogur.Hub.Domain.Entities;
using MediatR;

namespace Ogur.Hub.Application.Commands.LicensesCommands;

/// <summary>
/// Command to revoke (deactivate) a license.
/// </summary>
/// <param name="LicenseId">License identifier to revoke.</param>
public sealed record RevokeLicenseCommand(int LicenseId) : IRequest<bool>;

/// <summary>
/// Handler for revoking licenses.
/// </summary>
public sealed class RevokeLicenseCommandHandler : IRequestHandler<RevokeLicenseCommand, bool>
{
    private readonly IRepository<License, int> _licenseRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeLicenseCommandHandler"/> class.
    /// </summary>
    /// <param name="licenseRepository">License repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public RevokeLicenseCommandHandler(
        IRepository<License, int> licenseRepository,
        IUnitOfWork unitOfWork)
    {
        _licenseRepository = licenseRepository ?? throw new ArgumentNullException(nameof(licenseRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<bool> Handle(RevokeLicenseCommand request, CancellationToken cancellationToken)
    {
        var license = await _licenseRepository.GetByIdAsync(request.LicenseId, cancellationToken);
        if (license == null)
        {
            return false;
        }

        license.Deactivate();
        _licenseRepository.Update(license);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}