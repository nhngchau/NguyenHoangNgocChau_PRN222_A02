using Microsoft.AspNetCore.Mvc;
using NguyenHoangNgocChauRazorPages.Services;

namespace NguyenHoangNgocChauRazorPages.ViewComponents;

/// <summary>Renders the shared Staff analytics summary cards. Used by _StaffLayout so every Staff page shows the same dashboard view.</summary>
public class StaffStatsViewComponent : ViewComponent
{
    private readonly INewsService _news;
    private readonly ICategoryService _categories;

    public StaffStatsViewComponent(INewsService news, ICategoryService categories)
    {
        _news = news;
        _categories = categories;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var articles = await _news.SearchAsync(null);
        var categories = await _categories.SearchAsync(null);

        var model = new StaffStatsViewModel
        {
            TotalArticles = articles.Count,
            ActiveArticles = articles.Count(a => a.NewsStatus == true),
            InactiveArticles = articles.Count(a => a.NewsStatus != true),
            TotalCategories = categories.Count
        };
        return View(model);
    }
}

public class StaffStatsViewModel
{
    public int TotalArticles { get; set; }
    public int ActiveArticles { get; set; }
    public int InactiveArticles { get; set; }
    public int TotalCategories { get; set; }
}
