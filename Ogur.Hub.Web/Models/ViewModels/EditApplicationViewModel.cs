namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// Edit application page view model
/// </summary>
public sealed class EditApplicationViewModel : BasePageViewModel
{
    /// <summary>
    /// Application ID
    /// </summary>
    public required int ApplicationId { get; init; }
}