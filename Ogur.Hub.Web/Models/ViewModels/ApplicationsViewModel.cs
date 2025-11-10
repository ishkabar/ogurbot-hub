namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// Applications page view model
/// </summary>
public sealed class ApplicationsViewModel : BasePageViewModel
{
    /// <summary>
    /// List of applications
    /// </summary>
    public List<Services.ApplicationDto> Applications { get; init; } = new();
}

