using UPL.Domain.Entities;

namespace UPL.Infrastructure.Services;

public interface ICourseService
{
    Task<List<Course>> ListAsync(CancellationToken ct = default);
    Task<List<Course>> ListWithProgrammeAsync(CancellationToken ct = default);
    Task<Course?> GetAsync(int id, CancellationToken ct = default);
    Task<Course?> GetWithProgrammeAsync(int id, CancellationToken ct = default);
    Task CreateAsync(Course entity, CancellationToken ct = default);
    Task UpdateAsync(Course entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
