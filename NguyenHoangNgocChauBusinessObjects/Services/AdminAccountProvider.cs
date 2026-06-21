using Microsoft.Extensions.Options;

namespace NguyenHoangNgocChauRazorPages.Services;

/// <summary>
/// Application-wide, read-only source for the configured administrator account.
/// </summary>
public sealed class AdminAccountProvider
{
    public AdminAccountProvider(IOptions<AdminAccountOptions> options)
    {
        Account = options.Value;
    }

    public AdminAccountOptions Account { get; }
}
