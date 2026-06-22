using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NguyenHoangNgocChauRazorPages.Hubs;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Repositories;

namespace NguyenHoangNgocChauRazorPages.Services;

public class NewsService : INewsService
{
    private readonly IRepository<NewsArticle> _articles;
    private readonly IRepository<NewsTag> _newsTags;
    private readonly IHubContext<NewsHub> _hub;

    public NewsService(IRepository<NewsArticle> articles, IRepository<NewsTag> newsTags, IHubContext<NewsHub> hub)
    {
        _articles = articles;
        _newsTags = newsTags;
        _hub = hub;
    }

    public Task<List<NewsArticle>> SearchAsync(string? keyword, bool activeOnly = false, int? createdById = null, int? categoryId = null, int? tagId = null)
    {
        var query = IncludeDetails(_articles.Query());
        if (activeOnly)
        {
            query = query.Where(n => n.NewsStatus == true);
        }
        if (createdById.HasValue)
        {
            query = query.Where(n => n.CreatedByID == createdById.Value);
        }
        if (categoryId.HasValue)
        {
            query = query.Where(n => n.CategoryID == categoryId.Value);
        }
        if (tagId.HasValue)
        {
            query = query.Where(n => n.NewsTags.Any(nt => nt.TagID == tagId.Value));
        }
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(keyword)) || n.Headline.Contains(keyword) ||
                (n.NewsContent != null && n.NewsContent.Contains(keyword)) || (n.NewsSource != null && n.NewsSource.Contains(keyword)));
        }
        return query.OrderByDescending(n => n.CreatedDate).ToListAsync();
    }

    public Task<List<NewsArticle>> ReportAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = IncludeDetails(_articles.Query());
        if (startDate.HasValue)
        {
            query = query.Where(n => n.CreatedDate.HasValue && n.CreatedDate.Value.Date >= startDate.Value.Date);
        }
        if (endDate.HasValue)
        {
            query = query.Where(n => n.CreatedDate.HasValue && n.CreatedDate.Value.Date <= endDate.Value.Date);
        }
        return query.OrderByDescending(n => n.CreatedDate).ToListAsync();
    }

    public Task<List<ArticleState>> StatesAsync() => Task.FromResult(new List<ArticleState>());

    public Task<NewsArticle?> GetAsync(string id) => IncludeDetails(_articles.Query()).FirstOrDefaultAsync(n => n.NewsArticleID == id);

    public async Task SaveAsync(ArticleViewModel model, int staffId)
    {
        var isNew = string.IsNullOrWhiteSpace(model.NewsArticleID);
        NewsArticle article;
        if (isNew)
        {
            article = new NewsArticle
            {
                NewsArticleID = await NextIdAsync(),
                CreatedByID = checked((short)staffId),
                CreatedDate = DateTime.Now
            };
            await _articles.AddAsync(article);
        }
        else
        {
            article = await _articles.Query().Include(n => n.NewsTags).FirstAsync(n => n.NewsArticleID == model.NewsArticleID);
            article.UpdatedByID = checked((short)staffId);
            article.ModifiedDate = DateTime.Now;
            article.NewsTags.Clear();
        }

        article.NewsTitle = model.NewsTitle;
        article.Headline = model.Headline;
        article.NewsContent = model.NewsContent;
        article.NewsSource = model.NewsSource;
        article.CategoryID = checked((short)model.CategoryID);
        article.NewsStatus = model.NewsStatus;

        foreach (var tagId in model.TagIDs.Distinct())
        {
            article.NewsTags.Add(new NewsTag { NewsArticleID = article.NewsArticleID, TagID = tagId });
        }

        await _articles.SaveAsync();
        try { await BroadcastAsync(isNew ? "created" : "updated", article.NewsArticleID); }
        catch (Exception ex) { Console.Error.WriteLine($"[SignalR] BroadcastAsync failed: {ex.Message}"); }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var article = await _articles.Query().Include(n => n.NewsTags).FirstOrDefaultAsync(n => n.NewsArticleID == id);
        if (article == null)
        {
            return false;
        }

        foreach (var tag in article.NewsTags.ToList())
        {
            _newsTags.Delete(tag);
        }
        _articles.Delete(article);
        await _articles.SaveAsync();

        try { await _hub.Clients.All.SendAsync("NewsChanged", new NewsRealtimeDto { Action = "deleted", Id = id }); }
        catch (Exception ex) { Console.Error.WriteLine($"[SignalR] Delete broadcast failed: {ex.Message}"); }
        return true;
    }

    public async Task<bool> ToggleStatusAsync(string id, int staffId)
    {
        var article = await _articles.Query().FirstOrDefaultAsync(n => n.NewsArticleID == id);
        if (article == null)
        {
            return false;
        }

        article.NewsStatus = !article.NewsStatus;
        article.UpdatedByID = checked((short)staffId);
        article.ModifiedDate = DateTime.Now;
        await _articles.SaveAsync();

        await BroadcastAsync("updated", id);
        return true;
    }

    private async Task BroadcastAsync(string action, string id)
    {
        await _hub.Clients.All.SendAsync("NewsChanged", await CreateRealtimeDtoAsync(action, id));
    }

    private async Task<NewsRealtimeDto> CreateRealtimeDtoAsync(string action, string id)
    {
        var a = await IncludeDetails(_articles.Query().AsNoTracking()).FirstAsync(n => n.NewsArticleID == id);
        return new NewsRealtimeDto
        {
            Action = action,
            Id = a.NewsArticleID,
            Title = a.NewsTitle,
            Headline = a.Headline,
            Content = a.NewsContent,
            Source = a.NewsSource,
            CategoryId = a.CategoryID.GetValueOrDefault(),
            Category = a.Category?.CategoryName ?? "N/A",
            Created = a.CreatedDate?.ToString("dd/MM/yyyy") ?? "",
            Status = a.NewsStatus == true,
            StateId = a.ArticleStateID,
            Tags = string.Join(',', a.NewsTags.Select(t => t.TagID)),
            TagNames = a.NewsTags.Where(t => t.Tag != null).Select(t => t.Tag!.TagName).ToList()
        };
    }

    private IQueryable<NewsArticle> IncludeDetails(IQueryable<NewsArticle> query)
    {
        return query
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags)
            .ThenInclude(nt => nt.Tag);
    }

    private async Task<string> NextIdAsync()
    {
        var count = await _articles.Query().CountAsync() + 1;
        string id;
        do
        {
            id = $"N{count:000000000}";
            count++;
        }
        while (await _articles.Query().AnyAsync(n => n.NewsArticleID == id));

        return id;
    }
}
