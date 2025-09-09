using Microsoft.EntityFrameworkCore;
using UPL.Data;
using UPL.Domain.Entities;

namespace UPL.Infrastructure.Services;

public class ProgrammeService : IProgrammeService
{
    private readonly UplDbContext _db;
    public ProgrammeService(UplDbContext db) => _db = db;

    public Task<List<Programme>> ListAsync(CancellationToken ct = default)
        => _db.Programmes.AsNoTracking().ToListAsync(ct);

    public Task<Programme?> GetAsync(int id, CancellationToken ct = default)
        => _db.Programmes.FindAsync(new object[] { id }, ct).AsTask();

    public async Task CreateAsync(Programme entity, CancellationToken ct = default)
    {
        await _db.Programmes.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Programme entity, CancellationToken ct = default)
    {
        _db.Programmes.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Programmes.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _db.Programmes.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}

