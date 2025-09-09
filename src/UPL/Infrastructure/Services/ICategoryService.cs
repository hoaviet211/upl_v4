using UPL.Domain.Entities;

namespace UPL.Infrastructure.Services;

public interface ICategoryService
{
    Task<List<Category>> ListAsync(CancellationToken ct = default);
    Task<Category?> GetAsync(int id, CancellationToken ct = default);
    Task CreateAsync(Category entity, CancellationToken ct = default);
    Task UpdateAsync(Category entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

