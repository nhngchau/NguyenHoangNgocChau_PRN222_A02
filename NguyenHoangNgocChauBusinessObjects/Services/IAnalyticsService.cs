using NguyenHoangNgocChauRazorPages.Models.ViewModels;

namespace NguyenHoangNgocChauRazorPages.Services;

public interface IAnalyticsService
{
    Task<DashboardViewModel> GetDashboardAsync();
}
