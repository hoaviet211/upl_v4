using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UPL.Data;

namespace UPL.Infrastructure.Repositories;

public class EfRepository<T> : IGenericRepository<T> where T : class
{
    private readonly UplDbContext _db;
    private readonly DbSet<T> _set;

    public EfRepository(UplDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default) =>
        await _set.FindAsync(new[] { id }, ct);

    public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        var query = _set.AsQueryable();
        if (predicate != null) query = query.Where(predicate);
        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
    }

    public void Update(T entity) => _set.Update(entity);

    public void Delete(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

