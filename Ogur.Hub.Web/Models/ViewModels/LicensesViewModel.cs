namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// View model for licenses page
/// </summary>
public sealed class LicensesViewModel : BasePageViewModel
{
    /// <summary>
    /// List of licenses
    /// </summary>
    public List<Services.LicenseDto> Licenses { get; set; } = new();

    /// <summary>
    /// Optional application ID filter
    /// </summary>
    public int? ApplicationId { get; set; }
}