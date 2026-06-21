namespace NguyenHoangNgocChauRazorPages.Models;

public class ArticleLike
{
    public int ArticleLikeID { get; set; }
    public string NewsArticleID { get; set; } = string.Empty;
    public short AccountID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public NewsArticle? NewsArticle { get; set; }
    public SystemAccount? Account { get; set; }
}
