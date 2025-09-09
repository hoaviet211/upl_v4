using UPL.Domain.Entities;

namespace UPL.Infrastructure.Services;

public interface IProgrammeService
{
    Task<List<Programme>> ListAsync(CancellationToken ct = default);
    Task<Programme?> GetAsync(int id, CancellationToken ct = default);
    Task CreateAsync(Programme entity, CancellationToken ct = default);
    Task UpdateAsync(Programme entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

