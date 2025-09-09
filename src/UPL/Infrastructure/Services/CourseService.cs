using Microsoft.EntityFrameworkCore;
using UPL.Data;
using UPL.Domain.Entities;

namespace UPL.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly UplDbContext _db;
    public CourseService(UplDbContext db) => _db = db;

    public Task<List<Course>> ListAsync(CancellationToken ct = default)
        => _db.Courses.AsNoTracking().ToListAsync(ct);

    public Task<List<Course>> ListWithProgrammeAsync(CancellationToken ct = default)
        => _db.Courses.Include(c => c.Programme).AsNoTracking().ToListAsync(ct);

    public Task<Course?> GetAsync(int id, CancellationToken ct = default)
        => _db.Courses.FindAsync(new object[] { id }, ct).AsTask();

    public Task<Course?> GetWithProgrammeAsync(int id, CancellationToken ct = default)
        => _db.Courses.Include(c => c.Programme).FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task CreateAsync(Course entity, CancellationToken ct = default)
    {
        await _db.Courses.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Course entity, CancellationToken ct = default)
    {
        _db.Courses.Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Courses.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _db.Courses.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
