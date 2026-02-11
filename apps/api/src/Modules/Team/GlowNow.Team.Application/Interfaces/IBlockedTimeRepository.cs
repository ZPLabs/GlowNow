using GlowNow.Team.Domain.Entities;

namespace GlowNow.Team.Application.Interfaces;

/// <summary>
/// Repository interface for BlockedTime aggregate operations.
/// </summary>
public interface IBlockedTimeRepository
{
    /// <summary>
    /// Gets a blocked time by its unique identifier.
    /// </summary>
    Task<BlockedTime?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all blocked times for a staff member.
    /// </summary>
    Task<IReadOnlyList<BlockedTime>> GetByStaffProfileIdAsync(
        Guid staffProfileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all blocked times that apply to a specific date for a staff member.
    /// </summary>
    Task<IReadOnlyList<BlockedTime>> GetByStaffProfileIdAndDateAsync(
        Guid staffProfileId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all blocked times for a staff member within a date range.
    /// </summary>
    Task<IReadOnlyList<BlockedTime>> GetByStaffProfileIdAndDateRangeAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there's an overlapping blocked time.
    /// </summary>
    Task<bool> HasOverlappingBlockedTimeAsync(
        Guid staffProfileId,
        bool isRecurring,
        DayOfWeek? dayOfWeek,
        DateOnly? specificDate,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeBlockedTimeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new blocked time to the repository.
    /// </summary>
    void Add(BlockedTime blockedTime);

    /// <summary>
    /// Updates an existing blocked time in the repository.
    /// </summary>
    void Update(BlockedTime blockedTime);

    /// <summary>
    /// Removes a blocked time from the repository.
    /// </summary>
    void Remove(BlockedTime blockedTime);
}
