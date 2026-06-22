using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.News;

public class IndexModel(INewsService newsService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public List<NewsArticle> Articles { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Articles = await newsService.SearchAsync(SearchTerm, activeOnly: true);
    }

    /// <summary>AJAX endpoint used by the polling fallback on the guest page.</summary>
    public async Task<IActionResult> OnGetArticlesAsync()
    {
        var articles = await newsService.SearchAsync(null, activeOnly: true);

        var dtos = articles.Select(a => new NewsRealtimeDto
        {
            Action  = "updated",
            Id      = a.NewsArticleID,
            Title   = a.NewsTitle,
            Headline = a.Headline,
            Source  = a.NewsSource,
            Category = a.Category?.CategoryName ?? "Uncategorized",
            CategoryId = a.CategoryID ?? 0,
            Created = a.CreatedDate?.ToString("dd/MM/yyyy HH:mm") ?? "",
            Status  = a.NewsStatus == true,
            TagNames = a.NewsTags
                        .Where(nt => nt.Tag != null)
                        .Select(nt => nt.Tag!.TagName ?? "")
                        .ToList()
        });

        return new JsonResult(dtos);
    }
}
