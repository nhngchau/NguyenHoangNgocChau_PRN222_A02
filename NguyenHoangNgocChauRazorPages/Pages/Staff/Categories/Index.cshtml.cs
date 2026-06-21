using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NguyenHoangNgocChauRazorPages.Models;
using NguyenHoangNgocChauRazorPages.Services;
namespace NguyenHoangNgocChauRazorPages.Pages.Staff.Categories;
public class IndexModel(ICategoryService categoryService) : PageModel
{
    [BindProperty(SupportsGet=true)] public string? SearchTerm { get; set; }
    [BindProperty] public Category Input { get; set; } = new();
    public List<Category> Categories { get; private set; }=[]; public bool ShowModal { get; private set; }
    public async Task<IActionResult> OnGetAsync(){if(!HttpContext.Session.IsInRole("Staff"))return RedirectToPage("/Auth/Login");await LoadAsync();return Page();}
    public async Task<IActionResult> OnPostSaveAsync(){if(!HttpContext.Session.IsInRole("Staff"))return RedirectToPage("/Auth/Login");if(!ModelState.IsValid){ShowModal=true;await LoadAsync();return Page();}var isNew=Input.CategoryID==0;Category category;if(isNew)category=new();else category=await categoryService.GetAsync(Input.CategoryID)??throw new InvalidOperationException("Category not found.");category.CategoryName=Input.CategoryName.Trim();category.CategoryDescription=Input.CategoryDescription.Trim();category.IsActive=Input.IsActive;await categoryService.SaveAsync(category);TempData["Success"]=isNew?"Category created.":"Category updated.";return RedirectToPage(new{SearchTerm});}
    public async Task<IActionResult> OnPostDeleteAsync(int id){if(!HttpContext.Session.IsInRole("Staff"))return RedirectToPage("/Auth/Login");if(await categoryService.DeleteAsync(id))TempData["Success"]="Category deleted.";else TempData["Error"]="Category cannot be deleted because it is missing or is used by a news article.";return RedirectToPage(new{SearchTerm});}
    private async Task LoadAsync()=>Categories=await categoryService.SearchAsync(SearchTerm);
}
