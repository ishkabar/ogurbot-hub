using Ogur.Hub.Web.Models.ViewModels.Base;
using Ogur.Hub.Application.DTO;

namespace Ogur.Hub.Web.Models.ViewModels;

/// <summary>
/// Dashboard page view model
/// </summary>
public sealed class DashboardViewModel : BasePageViewModel
{
    /// <summary>
    /// Dashboard statistics
    /// </summary>
    public DashboardStatsDto Stats { get; init; } = new();
}