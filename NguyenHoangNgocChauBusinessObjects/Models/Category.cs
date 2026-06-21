using System.ComponentModel.DataAnnotations;

namespace NguyenHoangNgocChauRazorPages.Models;

public class Category
{
    public short CategoryID { get; set; }

    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string CategoryDescription { get; set; } = string.Empty;

    [Display(Name = "Parent Category")]
    public short? ParentCategoryID { get; set; }

    [Display(Name = "Active")]
    public bool? IsActive { get; set; } = true;

    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
