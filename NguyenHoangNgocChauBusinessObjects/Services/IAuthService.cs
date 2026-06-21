using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Services;

public interface IAuthService
{
    Task<AuthenticatedUser?> LoginAsync(string email, string password);
}
