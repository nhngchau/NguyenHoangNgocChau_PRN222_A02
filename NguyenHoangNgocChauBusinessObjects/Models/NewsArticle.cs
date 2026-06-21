using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NguyenHoangNgocChauRazorPages.Models;

public class NewsArticle
{
    [Required, StringLength(20)]
    public string NewsArticleID { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string? NewsTitle { get; set; }

    [Required, StringLength(250)]
    public string Headline { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime? CreatedDate { get; set; }

    [Required]
    public string? NewsContent { get; set; }

    [StringLength(200)]
    public string? NewsSource { get; set; }

    [NotMapped, StringLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public short? CategoryID { get; set; }

    [Display(Name = "Active")]
    public bool? NewsStatus { get; set; }

    [NotMapped] public int ArticleStateID { get; set; } = 3;

    [DataType(DataType.DateTime)]
    [NotMapped] public DateTime? ScheduledPublishDate { get; set; }

    public short? CreatedByID { get; set; }
    public short? UpdatedByID { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public Category? Category { get; set; }
    [NotMapped] public ArticleState? ArticleState { get; set; }
    public SystemAccount? CreatedBy { get; set; }
    public SystemAccount? UpdatedBy { get; set; }
    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    [NotMapped] public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    [NotMapped] public ICollection<ArticleLike> Likes { get; set; } = new List<ArticleLike>();
    [NotMapped] public ICollection<ArticleBookmark> Bookmarks { get; set; } = new List<ArticleBookmark>();
    [NotMapped] public ICollection<ArticleComment> Comments { get; set; } = new List<ArticleComment>();
}
