using Microsoft.EntityFrameworkCore;
using NguyenHoangNgocChauRazorPages.DAO;
using NguyenHoangNgocChauRazorPages.Data;
using NguyenHoangNgocChauRazorPages.Hubs;
using NguyenHoangNgocChauRazorPages.Repositories;
using NguyenHoangNgocChauRazorPages.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddDbContext<FUNewsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<AdminAccountOptions>(builder.Configuration.GetSection("AdminAccount"));
builder.Services.AddSingleton<AdminAccountProvider>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IDao<>), typeof(Dao<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IArticleWorkflowService, ArticleWorkflowService>();
builder.Services.AddScoped<IArticleInteractionService, ArticleInteractionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();
app.MapHub<NewsHub>("/newsHub");

app.Run();
