using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Services;

public interface ICurrentUserService
{
    AuthenticatedUser? User { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
