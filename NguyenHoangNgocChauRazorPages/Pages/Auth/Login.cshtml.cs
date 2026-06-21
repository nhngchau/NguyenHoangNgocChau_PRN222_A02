using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.Auth;

public class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty] public LoginViewModel Input { get; set; } = new();

    public IActionResult OnGet()
    {
        var user = HttpContext.Session.GetUser();
        return user is null ? Page() : RedirectToRolePage(user.Role);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var user = await authService.LoginAsync(Input.Email, Input.Password);
        if (user is null || user.Role == "Unknown")
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return Page();
        }
        HttpContext.Session.SetUser(user);
        return RedirectToRolePage(user.Role);
    }

    private IActionResult RedirectToRolePage(string role) => role switch
    {
        "Admin" => RedirectToPage("/Admin/Index"),
        "Staff" => RedirectToPage("/Staff/Index"),
        "Lecturer" => RedirectToPage("/News/Index"),
        _ => RedirectToPage("/Auth/Login")
    };
}
