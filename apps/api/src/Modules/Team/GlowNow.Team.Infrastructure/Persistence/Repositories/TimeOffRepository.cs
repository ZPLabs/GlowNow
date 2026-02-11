using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Team.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TimeOff aggregate.
/// </summary>
internal sealed class TimeOffRepository : ITimeOffRepository
{
    private readonly TeamDbContext _context;

    public TimeOffRepository(TeamDbContext context)
    {
        _context = context;
    }

    public async Task<TimeOff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TimeOffs
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TimeOff>> GetByStaffProfileIdAsync(
        Guid staffProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TimeOffs
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
        return await _context.TimeOffs
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
        return await _context.TimeOffs
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
        var query = _context.TimeOffs
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
        return await _context.TimeOffs
            .Where(t => t.BusinessId == businessId && t.Status == TimeOffStatus.Pending)
            .OrderBy(t => t.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public void Add(TimeOff timeOff)
    {
        _context.TimeOffs.Add(timeOff);
    }

    public void Update(TimeOff timeOff)
    {
        _context.TimeOffs.Update(timeOff);
    }

    public void Remove(TimeOff timeOff)
    {
        _context.TimeOffs.Remove(timeOff);
    }
}
