using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Services;
namespace NguyenHoangNgocChauRazorPages.Pages.Staff.Profile;
public class IndexModel(IAccountService accountService) : PageModel
{
    [BindProperty] public AccountViewModel Input { get; set; }=new();
    public async Task<IActionResult> OnGetAsync(){var user=HttpContext.Session.GetUser();if(user?.Role!="Staff")return RedirectToPage("/Auth/Login");var account=await accountService.GetAsync(user.AccountID);if(account is null)return RedirectToPage("/Auth/Login");Input=new AccountViewModel{AccountID=account.AccountID,AccountName=account.AccountName,AccountEmail=account.AccountEmail,AccountRole=account.AccountRole.GetValueOrDefault()};return Page();}
    public async Task<IActionResult> OnPostAsync(){var user=HttpContext.Session.GetUser();if(user?.Role!="Staff")return RedirectToPage("/Auth/Login");ModelState.Remove("Input.AccountRole");if(string.IsNullOrWhiteSpace(Input.AccountPassword))ModelState.Remove("Input.AccountPassword");if(!ModelState.IsValid)return Page();var account=await accountService.GetAsync(user.AccountID);if(account is null)return RedirectToPage("/Auth/Login");account.AccountName=Input.AccountName.Trim();account.AccountEmail=Input.AccountEmail.Trim();if(!string.IsNullOrWhiteSpace(Input.AccountPassword))account.AccountPassword=Input.AccountPassword;await accountService.SaveAsync(account);user.Name=account.AccountName;user.Email=account.AccountEmail;HttpContext.Session.SetUser(user);TempData["Success"]="Profile updated.";return RedirectToPage();}
}
