using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string? entityKey = null, string? description = null);
    Task<List<AuditLog>> LatestAsync(int take = 100);
}
