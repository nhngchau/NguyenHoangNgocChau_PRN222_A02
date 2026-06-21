using Microsoft.EntityFrameworkCore;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Repositories;

namespace NguyenHoangNgocChauRazorPages.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<SystemAccount> _accounts;
    private readonly AdminAccountProvider _adminProvider;

    public AuthService(IRepository<SystemAccount> accounts, AdminAccountProvider adminProvider)
    {
        _accounts = accounts;
        _adminProvider = adminProvider;
    }

    public async Task<AuthenticatedUser?> LoginAsync(string email, string password)
    {
        var admin = _adminProvider.Account;
        email = email.Trim();
        password = password.Trim();

        if (email.Equals(admin.Email, StringComparison.OrdinalIgnoreCase) && password == admin.Password)
        {
            return new AuthenticatedUser { AccountID = 0, Email = admin.Email, Name = admin.Name, Role = "Admin" };
        }

        if (email.Equals(admin.Email, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var account = await _accounts.Query()
            .FirstOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);

        if (account == null)
        {
            return null;
        }

        return new AuthenticatedUser
        {
            AccountID = account.AccountID,
            Email = account.AccountEmail,
            Name = account.AccountName,
            Role = account.AccountRole == 1 ? "Staff" : account.AccountRole == 2 ? "Lecturer" : "Unknown",
            StaffRoleValue = account.AccountRole
        };
    }
}
