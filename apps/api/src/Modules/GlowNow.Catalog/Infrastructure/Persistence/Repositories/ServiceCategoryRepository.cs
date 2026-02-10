using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Entities;
using GlowNow.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Catalog.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ServiceCategory aggregate.
/// </summary>
internal sealed class ServiceCategoryRepository : IServiceCategoryRepository
{
    private readonly AppDbContext _context;

    public ServiceCategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ServiceCategory>()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceCategory>> GetAllByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ServiceCategory>()
            .Where(c => c.BusinessId == businessId && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid businessId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ServiceCategory>()
            .AnyAsync(c => c.BusinessId == businessId
                        && c.Name.ToLower() == name.ToLower()
                        && !c.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid businessId,
        string name,
        Guid excludeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<ServiceCategory>()
            .AnyAsync(c => c.BusinessId == businessId
                        && c.Name.ToLower() == name.ToLower()
                        && c.Id != excludeId
                        && !c.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> HasServicesAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Service>()
            .AnyAsync(s => s.CategoryId == categoryId && !s.IsDeleted, cancellationToken);
    }

    public void Add(ServiceCategory category)
    {
        _context.Set<ServiceCategory>().Add(category);
    }

    public void Update(ServiceCategory category)
    {
        _context.Set<ServiceCategory>().Update(category);
    }

    public void Remove(ServiceCategory category)
    {
        _context.Set<ServiceCategory>().Remove(category);
    }
}
