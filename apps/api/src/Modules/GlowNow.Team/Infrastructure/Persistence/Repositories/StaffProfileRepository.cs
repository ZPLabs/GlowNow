using GlowNow.Shared.Infrastructure.Persistence;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for StaffProfile aggregate.
/// </summary>
internal sealed class StaffProfileRepository : IStaffProfileRepository
{
    private readonly AppDbContext _context;

    public StaffProfileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StaffProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted, cancellationToken);
    }

    public async Task<StaffProfile?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .Include(sp => sp.ServiceAssignments)
            .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<StaffProfile>> GetAllByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .Where(sp => sp.BusinessId == businessId && !sp.IsDeleted)
            .OrderBy(sp => sp.DisplayOrder)
            .ThenBy(sp => sp.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffProfile>> GetActiveByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .Where(sp => sp.BusinessId == businessId
                      && sp.Status == StaffStatus.Active
                      && !sp.IsDeleted)
            .OrderBy(sp => sp.DisplayOrder)
            .ThenBy(sp => sp.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffProfile>> GetByServiceIdAsync(
        Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .Include(sp => sp.ServiceAssignments)
            .Where(sp => sp.ServiceAssignments.Any(sa => sa.ServiceId == serviceId)
                      && sp.Status == StaffStatus.Active
                      && !sp.IsDeleted)
            .OrderBy(sp => sp.DisplayOrder)
            .ThenBy(sp => sp.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<StaffProfile?> GetByUserIdAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .FirstOrDefaultAsync(sp => sp.BusinessId == businessId
                                    && sp.UserId == userId
                                    && !sp.IsDeleted,
                cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(
        Guid businessId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<StaffProfile>()
            .AnyAsync(sp => sp.BusinessId == businessId
                         && sp.UserId == userId
                         && !sp.IsDeleted,
                cancellationToken);
    }

    public void Add(StaffProfile staffProfile)
    {
        _context.Set<StaffProfile>().Add(staffProfile);
    }

    public void Update(StaffProfile staffProfile)
    {
        _context.Set<StaffProfile>().Update(staffProfile);
    }

    public void Remove(StaffProfile staffProfile)
    {
        _context.Set<StaffProfile>().Remove(staffProfile);
    }
}
