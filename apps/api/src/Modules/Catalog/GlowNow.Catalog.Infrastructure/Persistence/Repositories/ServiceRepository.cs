using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Service aggregate.
/// </summary>
internal sealed class ServiceRepository : IServiceRepository
{
    private readonly CatalogDbContext _context;

    public ServiceRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Service>> GetAllByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Where(s => s.BusinessId == businessId && !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Service>> GetByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .Where(s => s.CategoryId == categoryId && !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid businessId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .AnyAsync(s => s.BusinessId == businessId
                        && s.Name.ToLower() == name.ToLower()
                        && !s.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid businessId,
        string name,
        Guid excludeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .AnyAsync(s => s.BusinessId == businessId
                        && s.Name.ToLower() == name.ToLower()
                        && s.Id != excludeId
                        && !s.IsDeleted,
                cancellationToken);
    }

    public void Add(Service service)
    {
        _context.Services.Add(service);
    }

    public void Update(Service service)
    {
        _context.Services.Update(service);
    }

    public void Remove(Service service)
    {
        _context.Services.Remove(service);
    }
}
