using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenHoangNgocChauRazorPages.Models;

public class SystemAccount
{
    public short AccountID { get; set; }

    [Required, StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(100)]
    public string AccountEmail { get; set; } = string.Empty;

    [Range(1, 2)]
    public int? AccountRole { get; set; }

    [Required, StringLength(100, MinimumLength = 6)]
    public string AccountPassword { get; set; } = string.Empty;

    [NotMapped, StringLength(30)]
    public string? PhoneNumber { get; set; }

    [NotMapped, StringLength(500)]
    public string? AvatarUrl { get; set; }

    [NotMapped, StringLength(500)]
    public string? Bio { get; set; }

    public ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
    public ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
    [NotMapped] public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    [NotMapped] public ICollection<ArticleLike> ArticleLikes { get; set; } = new List<ArticleLike>();
    [NotMapped] public ICollection<ArticleBookmark> ArticleBookmarks { get; set; } = new List<ArticleBookmark>();
    [NotMapped] public ICollection<ArticleComment> ArticleComments { get; set; } = new List<ArticleComment>();

    public string RoleName => AccountRole == 1 ? "Staff" : AccountRole == 2 ? "Lecturer" : "Unknown";
}
