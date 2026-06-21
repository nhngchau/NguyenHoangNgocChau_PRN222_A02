using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.Admin.Accounts;

public class IndexModel(IAccountService accountService) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
    [BindProperty] public AccountViewModel Input { get; set; } = new();
    public List<SystemAccount> Accounts { get; private set; } = [];
    public bool ShowModal { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!HttpContext.Session.IsInRole("Admin")) return RedirectToPage("/Auth/Login");
        await LoadAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!HttpContext.Session.IsInRole("Admin")) return RedirectToPage("/Auth/Login");
        var isNew = Input.AccountID == 0;
        if (!isNew) ModelState.Remove("Input.AccountPassword");
        if (!ModelState.IsValid || (Input.AccountRole != 1 && Input.AccountRole != 2))
        {
            if (Input.AccountRole != 1 && Input.AccountRole != 2) ModelState.AddModelError("Input.AccountRole", "Role must be Staff or Lecturer.");
            ShowModal = true; await LoadAsync(); return Page();
        }

        SystemAccount account;
        if (isNew)
        {
            account = new SystemAccount();
        }
        else
        {
            account = await accountService.GetAsync(Input.AccountID) ?? throw new InvalidOperationException("Account not found.");
        }
        account.AccountName = Input.AccountName.Trim(); account.AccountEmail = Input.AccountEmail.Trim(); account.AccountRole = Input.AccountRole;
        if (isNew || !string.IsNullOrWhiteSpace(Input.AccountPassword)) account.AccountPassword = Input.AccountPassword;
        await accountService.SaveAsync(account);
        TempData["Success"] = isNew ? "Account created." : "Account updated.";
        return RedirectToPage(new { SearchTerm });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (!HttpContext.Session.IsInRole("Admin")) return RedirectToPage("/Auth/Login");
        if (await accountService.DeleteAsync(id))
        {
            TempData["Success"] = "Account deleted.";
        }
        else
        {
            TempData["Error"] = "Account cannot be deleted because it is missing or is referenced by news.";
        }
        return RedirectToPage(new { SearchTerm });
    }

    private async Task LoadAsync() => Accounts = await accountService.SearchAsync(SearchTerm);
}
