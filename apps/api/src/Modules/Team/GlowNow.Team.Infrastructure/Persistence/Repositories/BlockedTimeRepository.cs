using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Team.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for BlockedTime aggregate.
/// </summary>
internal sealed class BlockedTimeRepository : IBlockedTimeRepository
{
    private readonly TeamDbContext _context;

    public BlockedTimeRepository(TeamDbContext context)
    {
        _context = context;
    }

    public async Task<BlockedTime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BlockedTimes
            .FirstOrDefaultAsync(bt => bt.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BlockedTime>> GetByStaffProfileIdAsync(
        Guid staffProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.BlockedTimes
            .Where(bt => bt.StaffProfileId == staffProfileId)
            .OrderBy(bt => bt.IsRecurring)
            .ThenBy(bt => bt.RecurringDayOfWeek)
            .ThenBy(bt => bt.SpecificDate)
            .ThenBy(bt => bt.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockedTime>> GetByStaffProfileIdAndDateAsync(
        Guid staffProfileId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var dayOfWeek = date.DayOfWeek;

        return await _context.BlockedTimes
            .Where(bt => bt.StaffProfileId == staffProfileId
                      && ((bt.IsRecurring && bt.RecurringDayOfWeek == dayOfWeek)
                          || (!bt.IsRecurring && bt.SpecificDate == date)))
            .OrderBy(bt => bt.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlockedTime>> GetByStaffProfileIdAndDateRangeAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        // For date range queries, we need to include:
        // 1. All recurring blocked times (they might apply to any day in the range)
        // 2. One-time blocked times that fall within the range
        return await _context.BlockedTimes
            .Where(bt => bt.StaffProfileId == staffProfileId
                      && (bt.IsRecurring
                          || (bt.SpecificDate >= startDate && bt.SpecificDate <= endDate)))
            .OrderBy(bt => bt.IsRecurring)
            .ThenBy(bt => bt.SpecificDate)
            .ThenBy(bt => bt.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingBlockedTimeAsync(
        Guid staffProfileId,
        bool isRecurring,
        DayOfWeek? dayOfWeek,
        DateOnly? specificDate,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeBlockedTimeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.BlockedTimes
            .Where(bt => bt.StaffProfileId == staffProfileId
                      && bt.StartTime < endTime
                      && bt.EndTime > startTime);

        if (isRecurring)
        {
            query = query.Where(bt => bt.IsRecurring && bt.RecurringDayOfWeek == dayOfWeek);
        }
        else
        {
            query = query.Where(bt =>
                (!bt.IsRecurring && bt.SpecificDate == specificDate)
                || (bt.IsRecurring && bt.RecurringDayOfWeek == specificDate!.Value.DayOfWeek));
        }

        if (excludeBlockedTimeId.HasValue)
        {
            query = query.Where(bt => bt.Id != excludeBlockedTimeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public void Add(BlockedTime blockedTime)
    {
        _context.BlockedTimes.Add(blockedTime);
    }

    public void Update(BlockedTime blockedTime)
    {
        _context.BlockedTimes.Update(blockedTime);
    }

    public void Remove(BlockedTime blockedTime)
    {
        _context.BlockedTimes.Remove(blockedTime);
    }
}
