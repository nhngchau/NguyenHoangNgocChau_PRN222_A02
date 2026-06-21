using Microsoft.EntityFrameworkCore;
using NguyenHoangNgocChauRazorPages.Data;
using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Services;

public class ArticleInteractionService : IArticleInteractionService
{
    private readonly FUNewsDbContext _context;

    public ArticleInteractionService(FUNewsDbContext context)
    {
        _context = context;
    }

    public Task<int> LikeCountAsync(string articleId) => _context.ArticleLikes.CountAsync(l => l.NewsArticleID == articleId);

    public Task<int> BookmarkCountAsync(string articleId) => _context.ArticleBookmarks.CountAsync(b => b.NewsArticleID == articleId);

    public Task<List<NewsArticle>> BookmarkedArticlesAsync(int accountId)
    {
        return _context.NewsArticles
            .Where(n => n.Bookmarks.Any(b => b.AccountID == accountId))
            .Include(n => n.Category)
            .Include(n => n.ArticleState)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags)
            .ThenInclude(nt => nt.Tag)
            .OrderByDescending(n => n.Bookmarks.Where(b => b.AccountID == accountId).Max(b => b.CreatedAt))
            .ToListAsync();
    }

    public Task<List<ArticleComment>> CommentsAsync(string articleId)
    {
        return _context.ArticleComments
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
            .Where(c => c.NewsArticleID == articleId && c.ParentCommentID == null && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task ToggleLikeAsync(string articleId, int accountId)
    {
        var like = await _context.ArticleLikes.FirstOrDefaultAsync(l => l.NewsArticleID == articleId && l.AccountID == accountId);
        if (like == null)
        {
            _context.ArticleLikes.Add(new ArticleLike { NewsArticleID = articleId, AccountID = checked((short)accountId) });
        }
        else
        {
            _context.ArticleLikes.Remove(like);
        }
        await _context.SaveChangesAsync();
    }

    public async Task ToggleBookmarkAsync(string articleId, int accountId)
    {
        var bookmark = await _context.ArticleBookmarks.FirstOrDefaultAsync(b => b.NewsArticleID == articleId && b.AccountID == accountId);
        if (bookmark == null)
        {
            _context.ArticleBookmarks.Add(new ArticleBookmark { NewsArticleID = articleId, AccountID = checked((short)accountId) });
        }
        else
        {
            _context.ArticleBookmarks.Remove(bookmark);
        }
        await _context.SaveChangesAsync();
    }

    public async Task AddCommentAsync(string articleId, int? accountId, string displayName, string content, int? parentCommentId)
    {
        _context.ArticleComments.Add(new ArticleComment
        {
            NewsArticleID = articleId,
            AccountID = !accountId.HasValue || accountId.Value == 0 ? null : checked((short)accountId.Value),
            DisplayName = displayName.Trim(),
            Content = content.Trim(),
            ParentCommentID = parentCommentId,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }
}
