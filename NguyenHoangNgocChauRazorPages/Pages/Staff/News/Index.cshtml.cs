using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using NguyenHoangNgocChauRazorPages.Hubs;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Models.ViewModels;
using NguyenHoangNgocChauRazorPages.Services;
namespace NguyenHoangNgocChauRazorPages.Pages.Staff.News;
public class IndexModel(INewsService newsService, ICategoryService categoryService, ITagService tagService, IHubContext<NewsHub> hubContext) : PageModel
{
    [BindProperty(SupportsGet=true)] public string? SearchTerm { get; set; }
    [BindProperty] public ArticleViewModel Input { get; set; }=new();
    public List<NewsArticle> Articles { get; private set; }=[]; public List<Category> Categories { get; private set; }=[]; public List<Tag> Tags { get; private set; }=[]; public bool ShowModal { get; private set; }
    public async Task<IActionResult> OnGetAsync(){if(!IsStaff())return RedirectToPage("/Auth/Login");await LoadAsync();return Page();}
    public async Task<IActionResult> OnPostSaveAsync(){if(!IsStaff())return RedirectToPage("/Auth/Login");if(Input.CategoryID<=0)ModelState.AddModelError("Input.CategoryID","Category is required.");if(!ModelState.IsValid){ShowModal=true;await LoadAsync();return Page();}var staffId=HttpContext.Session.GetUser()!.AccountID;var action=string.IsNullOrWhiteSpace(Input.NewsArticleID)?"created":"updated";await newsService.SaveAsync(Input,staffId);await hubContext.Clients.All.SendAsync("NewsChanged",action);TempData["Success"]=action=="created"?"News created.":"News updated.";return RedirectToPage(new{SearchTerm});}
    public async Task<IActionResult> OnPostDeleteAsync(string id){if(!IsStaff())return RedirectToPage("/Auth/Login");if(await newsService.DeleteAsync(id)){await hubContext.Clients.All.SendAsync("NewsChanged","deleted");TempData["Success"]="News deleted.";}else TempData["Error"]="News article was not found.";return RedirectToPage(new{SearchTerm});}
    private bool IsStaff()=>HttpContext.Session.IsInRole("Staff");
    private async Task LoadAsync(){Articles=await newsService.SearchAsync(SearchTerm);Categories=await categoryService.SearchAsync(null);Tags=await tagService.AllAsync();}
}
