using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.Admin;

public class IndexModel : PageModel
{
    public IActionResult OnGet() => HttpContext.Session.IsInRole("Admin") ? Page() : RedirectToPage("/Auth/Login");
}
