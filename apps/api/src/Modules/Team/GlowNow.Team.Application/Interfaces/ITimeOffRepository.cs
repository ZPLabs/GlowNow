using GlowNow.Team.Domain.Entities;

namespace GlowNow.Team.Application.Interfaces;

/// <summary>
/// Repository interface for TimeOff aggregate operations.
/// </summary>
public interface ITimeOffRepository
{
    /// <summary>
    /// Gets a time off request by its unique identifier.
    /// </summary>
    Task<TimeOff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all time off requests for a staff member.
    /// </summary>
    Task<IReadOnlyList<TimeOff>> GetByStaffProfileIdAsync(
        Guid staffProfileId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all time off requests for a staff member within a date range.
    /// </summary>
    Task<IReadOnlyList<TimeOff>> GetByStaffProfileIdAndDateRangeAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all approved time off for a staff member within a date range.
    /// </summary>
    Task<IReadOnlyList<TimeOff>> GetApprovedByStaffProfileIdAndDateRangeAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if there's an overlapping approved time off for the given period.
    /// </summary>
    Task<bool> HasOverlappingApprovedTimeOffAsync(
        Guid staffProfileId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeTimeOffId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all pending time off requests for a business.
    /// </summary>
    Task<IReadOnlyList<TimeOff>> GetPendingByBusinessIdAsync(
        Guid businessId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new time off request to the repository.
    /// </summary>
    void Add(TimeOff timeOff);

    /// <summary>
    /// Updates an existing time off request in the repository.
    /// </summary>
    void Update(TimeOff timeOff);

    /// <summary>
    /// Removes a time off request from the repository.
    /// </summary>
    void Remove(TimeOff timeOff);
}
