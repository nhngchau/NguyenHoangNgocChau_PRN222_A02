using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Services;

public interface IAccountService
{
    Task<List<SystemAccount>> SearchAsync(string? keyword);
    Task<SystemAccount?> GetAsync(int id);
    Task SaveAsync(SystemAccount account);
    Task<bool> DeleteAsync(int id);
}
