using Microsoft.EntityFrameworkCore;
using UPL.Data;
using UPL.Domain.Entities;

namespace UPL.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly UplDbContext _db;
    public CategoryService(UplDbContext db) => _db = db;

    public Task<List<Category>> ListAsync(CancellationToken ct = default)
        => _db.Categories.AsNoTracking().ToListAsync(ct);

    public Task<Category?> GetAsync(int id, CancellationToken ct = default)
        => _db.Categories.FindAsync(new object[] { id }, ct).AsTask();

    public async Task CreateAsync(Category entity, CancellationToken ct = default)
    {
        await _db.Categories.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Category entity, CancellationToken ct = default)
    {
        _db.Categories.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _db.Categories.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

