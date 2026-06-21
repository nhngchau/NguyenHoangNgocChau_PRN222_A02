using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.Staff;

public class IndexModel : PageModel
{
    public IActionResult OnGet() => HttpContext.Session.IsInRole("Staff") ? Page() : RedirectToPage("/Auth/Login");
}
