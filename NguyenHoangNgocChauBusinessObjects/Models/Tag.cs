using System.ComponentModel.DataAnnotations;

namespace NguyenHoangNgocChauRazorPages.Models;

public class Tag
{
    public int TagID { get; set; }

    [Required, StringLength(100)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Note { get; set; }

    public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
