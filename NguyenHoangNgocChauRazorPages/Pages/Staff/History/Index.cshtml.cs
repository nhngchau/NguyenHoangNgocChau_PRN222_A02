using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Services;
namespace NguyenHoangNgocChauRazorPages.Pages.Staff.History;
public class IndexModel(INewsService newsService) : PageModel
{
    [BindProperty(SupportsGet=true)] public string? SearchTerm { get; set; }
    public List<NewsArticle> Articles { get; private set; }=[];
    public async Task<IActionResult> OnGetAsync(){var user=HttpContext.Session.GetUser();if(user?.Role!="Staff")return RedirectToPage("/Auth/Login");Articles=await newsService.SearchAsync(SearchTerm,createdById:user.AccountID);return Page();}
}
