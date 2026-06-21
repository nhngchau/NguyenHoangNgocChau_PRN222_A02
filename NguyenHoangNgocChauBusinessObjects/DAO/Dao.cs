using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NguyenHoangNgocChauRazorPages.Data;

namespace NguyenHoangNgocChauRazorPages.DAO;

public class Dao<T>(FUNewsDbContext context) : IDao<T> where T : class
{
    public IQueryable<T> Query() => context.Set<T>();
    public Task<List<T>> GetAllAsync() => context.Set<T>().ToListAsync();
    public async Task<T?> GetByIdAsync(params object[] id) => await context.Set<T>().FindAsync(id);
    public Task AddAsync(T entity) => context.Set<T>().AddAsync(entity).AsTask();
    public void Update(T entity) => context.Set<T>().Update(entity);
    public void Delete(T entity) => context.Set<T>().Remove(entity);
    public Task SaveAsync() => context.SaveChangesAsync();
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => context.Set<T>().AnyAsync(predicate);
}
