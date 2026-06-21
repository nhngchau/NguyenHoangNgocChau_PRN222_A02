using Microsoft.EntityFrameworkCore;
using NguyenHoangNgocChauRazorPages.Models;

namespace NguyenHoangNgocChauRazorPages.Data;

public class FUNewsDbContext : DbContext
{
    public FUNewsDbContext(DbContextOptions<FUNewsDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SystemAccount> SystemAccounts => Set<SystemAccount>();
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<NewsTag> NewsTags => Set<NewsTag>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ArticleState> ArticleStates => Set<ArticleState>();
    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();
    public DbSet<ArticleLike> ArticleLikes => Set<ArticleLike>();
    public DbSet<ArticleBookmark> ArticleBookmarks => Set<ArticleBookmark>();
    public DbSet<ArticleComment> ArticleComments => Set<ArticleComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // These are application-only legacy types, not tables in FUNewsManagement.sql.
        modelBuilder.Ignore<AuditLog>();
        modelBuilder.Ignore<ArticleState>();
        modelBuilder.Ignore<ApprovalHistory>();
        modelBuilder.Ignore<ArticleLike>();
        modelBuilder.Ignore<ArticleBookmark>();
        modelBuilder.Ignore<ArticleComment>();

        modelBuilder.Entity<Category>().ToTable("Category").HasKey(c => c.CategoryID);
        modelBuilder.Entity<SystemAccount>().ToTable("SystemAccount").HasKey(a => a.AccountID);
        modelBuilder.Entity<NewsArticle>().ToTable("NewsArticle").HasKey(n => n.NewsArticleID);
        modelBuilder.Entity<Tag>().ToTable("Tag").HasKey(t => t.TagID);
        modelBuilder.Entity<NewsTag>().ToTable("NewsTag").HasKey(nt => new { nt.NewsArticleID, nt.TagID });
        // Exact mappings from FUNewsManagement.sql.
        modelBuilder.Entity<SystemAccount>().Property(a => a.AccountID).HasColumnType("smallint");
        modelBuilder.Entity<SystemAccount>().Property(a => a.AccountRole).HasColumnType("int");
        modelBuilder.Entity<Category>().Property(c => c.CategoryID).HasColumnType("smallint");
        modelBuilder.Entity<Category>().Property(c => c.CategoryDescription).HasColumnName("CategoryDesciption");
        modelBuilder.Entity<Category>().Property(c => c.ParentCategoryID).HasColumnType("smallint");
        modelBuilder.Entity<Category>().Property(c => c.IsActive).HasColumnType("bit");
        modelBuilder.Entity<NewsArticle>().Property(n => n.CategoryID).HasColumnType("smallint");
        modelBuilder.Entity<NewsArticle>().Property(n => n.CreatedByID).HasColumnType("smallint");
        modelBuilder.Entity<NewsArticle>().Property(n => n.UpdatedByID).HasColumnType("smallint");

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.Category)
            .WithMany(c => c.NewsArticles)
            .HasForeignKey(n => n.CategoryID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.CreatedBy)
            .WithMany(a => a.CreatedNewsArticles)
            .HasForeignKey(n => n.CreatedByID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsArticle>()
            .HasOne(n => n.UpdatedBy)
            .WithMany(a => a.UpdatedNewsArticles)
            .HasForeignKey(n => n.UpdatedByID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.NewsArticle)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(nt => nt.NewsArticleID);

        modelBuilder.Entity<NewsTag>()
            .HasOne(nt => nt.Tag)
            .WithMany(t => t.NewsTags)
            .HasForeignKey(nt => nt.TagID);

    }
}
