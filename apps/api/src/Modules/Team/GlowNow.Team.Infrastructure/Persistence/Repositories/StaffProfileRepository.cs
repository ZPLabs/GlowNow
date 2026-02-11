using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Team.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for StaffProfile aggregate.
/// </summary>
internal sealed class StaffProfileRepository : IStaffProfileRepository
{
    private readonly TeamDbContext _context;

    public StaffProfileRepository(TeamDbContext context)
    {
        _context = context;
    }

    public async Task<StaffProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StaffProfiles
            .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted, cancellationToken);
    }

    public async Task<StaffProfile?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.StaffProfiles
            .Include(sp => sp.ServiceAssignments)
            .FirstOrDefaultAsync(sp => sp.Id == id && !sp.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<StaffProfile>> GetAllByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StaffProfiles
            .Where(sp => sp.BusinessId == businessId && !sp.IsDeleted)
            .OrderBy(sp => sp.DisplayOrder)
            .ThenBy(sp => sp.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffProfile>> GetActiveByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.StaffProfiles
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
        return await _context.StaffProfiles
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
        return await _context.StaffProfiles
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
        return await _context.StaffProfiles
            .AnyAsync(sp => sp.BusinessId == businessId
                         && sp.UserId == userId
                         && !sp.IsDeleted,
                cancellationToken);
    }

    public void Add(StaffProfile staffProfile)
    {
        _context.StaffProfiles.Add(staffProfile);
    }

    public void Update(StaffProfile staffProfile)
    {
        _context.StaffProfiles.Update(staffProfile);
    }

    public void Remove(StaffProfile staffProfile)
    {
        _context.StaffProfiles.Remove(staffProfile);
    }
}
