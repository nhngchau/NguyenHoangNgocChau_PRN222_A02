using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.Admin.Reports;

public class IndexModel(INewsService newsService) : PageModel
{
    [BindProperty(SupportsGet = true)] public DateTime? StartDate { get; set; }
    [BindProperty(SupportsGet = true)] public DateTime? EndDate { get; set; }
    public List<NewsArticle> Articles { get; private set; } = [];
    public bool HasFilter => StartDate.HasValue || EndDate.HasValue;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!HttpContext.Session.IsInRole("Admin")) return RedirectToPage("/Auth/Login");
        if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
        {
            TempData["Error"] = "Start Date must be earlier than or equal to End Date.";
            return RedirectToPage();
        }
        if (HasFilter) Articles = await newsService.ReportAsync(StartDate, EndDate);
        return Page();
    }
}
