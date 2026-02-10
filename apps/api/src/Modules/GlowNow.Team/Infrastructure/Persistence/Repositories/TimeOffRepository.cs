using GlowNow.Shared.Infrastructure.Persistence;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TimeOff aggregate.
/// </summary>
internal sealed class TimeOffRepository : ITimeOffRepository
{
    private readonly AppDbContext _context;

    public TimeOffRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TimeOff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TimeOff>()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TimeOff>> GetByStaffProfileIdAsync(
        Guid staffProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TimeOff>()
            .Where(t => t.StaffProfileId == staffProfileId)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeOff>> GetByStaffProfileIdAndDateRangeAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TimeOff>()
            .Where(t => t.StaffProfileId == staffProfileId
                     && t.StartDate <= endDate
                     && t.EndDate >= startDate)
            .OrderBy(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeOff>> GetApprovedByStaffProfileIdAndDateRangeAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TimeOff>()
            .Where(t => t.StaffProfileId == staffProfileId
                     && t.Status == TimeOffStatus.Approved
                     && t.StartDate <= endDate
                     && t.EndDate >= startDate)
            .OrderBy(t => t.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingApprovedTimeOffAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeTimeOffId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TimeOff>()
            .Where(t => t.StaffProfileId == staffProfileId
                     && t.Status == TimeOffStatus.Approved
                     && t.StartDate <= endDate
                     && t.EndDate >= startDate);

        if (excludeTimeOffId.HasValue)
        {
            query = query.Where(t => t.Id != excludeTimeOffId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimeOff>> GetPendingByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<TimeOff>()
            .Where(t => t.BusinessId == businessId && t.Status == TimeOffStatus.Pending)
            .OrderBy(t => t.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void Add(TimeOff timeOff)
    {
        _context.Set<TimeOff>().Add(timeOff);
    }

    public void Update(TimeOff timeOff)
    {
        _context.Set<TimeOff>().Update(timeOff);
    }

    public void Remove(TimeOff timeOff)
    {
        _context.Set<TimeOff>().Remove(timeOff);
    }
}
