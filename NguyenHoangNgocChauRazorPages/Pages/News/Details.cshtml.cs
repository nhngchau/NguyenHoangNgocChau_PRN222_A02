using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.Pages.News;

public class DetailsModel(INewsService newsService) : PageModel
{
    public NewsArticle Article { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var article = await newsService.GetAsync(id);
        if (article is null || article.NewsStatus != true)
        {
            return NotFound();
        }

        Article = article;
        return Page();
    }
}
