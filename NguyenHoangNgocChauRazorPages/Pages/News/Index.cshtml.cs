using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
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
}
