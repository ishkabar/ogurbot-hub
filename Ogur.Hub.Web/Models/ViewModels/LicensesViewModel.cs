using Ogur.Hub.Web.Models.ViewModels.Base;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for licenses page
/// </summary>
public sealed class LicensesViewModel : BasePageViewModel
{
    /// <summary>
    /// List of licenses
    /// </summary>
    public List<LicenseDto> Licenses { get; set; } = new();

    /// <summary>
    /// Optional application ID filter
    /// </summary>
    public int? ApplicationId { get; set; }
}

/// <summary>
/// View model for license edit page
/// </summary>
public class LicenseEditViewModel : BaseEditViewModel
{
    /// <summary>
    /// License ID to edit
    /// </summary>
    public int LicenseId { get; set; }
}

/// <summary>
/// View model for license details page
/// </summary>
public class LicenseDetailsViewModel : BaseDetailsViewModel
{
    /// <summary>
    /// License ID to load
    /// </summary>
    public int LicenseId { get; set; }
}

public class LicenseCreateViewModel : BasePageViewModel
{
    
}