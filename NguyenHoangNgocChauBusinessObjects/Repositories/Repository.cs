using System.Linq.Expressions;
using NguyenHoangNgocChauRazorPages.DAO;

namespace NguyenHoangNgocChauRazorPages.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IDao<T> _dao;

    public Repository(IDao<T> dao)
    {
        _dao = dao;
    }

    public IQueryable<T> Query() => _dao.Query();

    public Task<List<T>> GetAllAsync() => _dao.GetAllAsync();

    public Task<T?> GetByIdAsync(params object[] id) => _dao.GetByIdAsync(id);

    public Task AddAsync(T entity) => _dao.AddAsync(entity);

    public void Update(T entity) => _dao.Update(entity);

    public void Delete(T entity) => _dao.Delete(entity);

    public Task SaveAsync() => _dao.SaveAsync();

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => _dao.AnyAsync(predicate);
}
