using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Services;
namespace NguyenHoangNgocChauRazorPages.Pages.Staff.News;
public class IndexModel(INewsService newsService, ICategoryService categoryService, ITagService tagService) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? SearchTerm { get; set; }
    [BindProperty] public ArticleViewModel Input { get; set; } = new();
    public List<NewsArticle> Articles { get; private set; } = [];
    public List<Category> Categories { get; private set; } = [];
    public List<Tag> Tags { get; private set; } = [];
    public bool ShowModal { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!IsStaff()) return RedirectToPage("/Auth/Login");
        await LoadAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!IsStaff())
        {
            if (IsAjax()) return new JsonResult(new { success = false, message = "Unauthorized" }) { StatusCode = 401 };
            return RedirectToPage("/Auth/Login");
        }

        if (Input.CategoryID <= 0)
            ModelState.AddModelError("Input.CategoryID", "Category is required.");

        if (!ModelState.IsValid)
        {
            if (IsAjax())
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return new JsonResult(new { success = false, errors });
            }
            ShowModal = true;
            await LoadAsync();
            return Page();
        }

        var staffId = HttpContext.Session.GetUser()!.AccountID;
        var action = string.IsNullOrWhiteSpace(Input.NewsArticleID) ? "created" : "updated";
        await newsService.SaveAsync(Input, staffId);
        var message = action == "created" ? "News created successfully." : "News updated successfully.";

        if (IsAjax())
            return new JsonResult(new { success = true, message });

        TempData["Success"] = message;
        return RedirectToPage(new { SearchTerm });
    }

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        if (!IsStaff())
        {
            if (IsAjax()) return new JsonResult(new { success = false, message = "Unauthorized" }) { StatusCode = 401 };
            return RedirectToPage("/Auth/Login");
        }

        var deleted = await newsService.DeleteAsync(id);
        if (IsAjax())
            return new JsonResult(new { success = deleted, message = deleted ? "News deleted successfully." : "News article was not found." });

        if (deleted) TempData["Success"] = "News deleted.";
        else TempData["Error"] = "News article was not found.";
        return RedirectToPage(new { SearchTerm });
    }

    private bool IsStaff() => HttpContext.Session.IsInRole("Staff");
    private bool IsAjax() => Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    private async Task LoadAsync()
    {
        Articles = await newsService.SearchAsync(SearchTerm);
        Categories = await categoryService.SearchAsync(null);
        Tags = await tagService.AllAsync();
    }
}
