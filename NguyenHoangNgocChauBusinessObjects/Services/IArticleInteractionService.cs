using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Services;

public interface IArticleInteractionService
{
    Task<int> LikeCountAsync(string articleId);
    Task<int> BookmarkCountAsync(string articleId);
    Task<List<NewsArticle>> BookmarkedArticlesAsync(int accountId);
    Task<List<ArticleComment>> CommentsAsync(string articleId);
    Task ToggleLikeAsync(string articleId, int accountId);
    Task ToggleBookmarkAsync(string articleId, int accountId);
    Task AddCommentAsync(string articleId, int? accountId, string displayName, string content, int? parentCommentId);
}
