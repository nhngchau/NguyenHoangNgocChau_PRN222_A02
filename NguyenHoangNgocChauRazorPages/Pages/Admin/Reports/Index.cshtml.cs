using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.Admin.Reports;

public class IndexModel(INewsService newsService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }

    public List<NewsArticle> Articles { get; private set; } = [];

    // True once a valid period has been submitted and the report produced.
    public bool Generated { get; private set; }

    // Summary statistics for the selected period.
    public int TotalCount => Articles.Count;
    public int ActiveCount => Articles.Count(a => a.NewsStatus == true);
    public int InactiveCount => TotalCount - ActiveCount;

    public async Task<IActionResult> OnGetAsync()
    {
        if (!HttpContext.Session.IsInRole("Admin"))
        {
            return RedirectToPage("/Auth/Login");
        }

        // Initial load (no input yet) — just show the empty filter form.
        if (!StartDate.HasValue && !EndDate.HasValue)
        {
            return Page();
        }

        // A period report requires BOTH dates.
        if (!StartDate.HasValue)
        {
            ModelState.AddModelError(nameof(StartDate), "Please choose a start date.");
        }
        if (!EndDate.HasValue)
        {
            ModelState.AddModelError(nameof(EndDate), "Please choose an end date.");
        }
        if (StartDate.HasValue && EndDate.HasValue && StartDate > EndDate)
        {
            ModelState.AddModelError(nameof(EndDate), "End date must be on or after the start date.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // ReportAsync filters by CreatedDate within [StartDate, EndDate] and sorts descending.
        Articles = await newsService.ReportAsync(StartDate, EndDate);
        Generated = true;
        return Page();
    }
}
