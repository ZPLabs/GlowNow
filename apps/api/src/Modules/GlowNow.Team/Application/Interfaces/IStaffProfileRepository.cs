namespace GlowNow.Team.Application.Interfaces;

/// <summary>
/// Repository interface for StaffProfile aggregate operations.
/// </summary>
public interface IStaffProfileRepository
{
    /// <summary>
    /// Gets a staff profile by its unique identifier.
    /// </summary>
    Task<StaffProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a staff profile by its unique identifier with service assignments loaded.
    /// </summary>
    Task<StaffProfile?> GetByIdWithServicesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all staff profiles for a business.
    /// </summary>
    Task<IReadOnlyList<StaffProfile>> GetAllByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active staff profiles for a business.
    /// </summary>
    Task<IReadOnlyList<StaffProfile>> GetActiveByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets staff profiles that can perform a specific service.
    /// </summary>
    Task<IReadOnlyList<StaffProfile>> GetByServiceIdAsync(Guid serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a staff profile by user ID within a business.
    /// </summary>
    Task<StaffProfile?> GetByUserIdAsync(Guid businessId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a staff profile exists for a user in a business.
    /// </summary>
    Task<bool> ExistsByUserIdAsync(Guid businessId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new staff profile to the repository.
    /// </summary>
    void Add(StaffProfile staffProfile);

    /// <summary>
    /// Updates an existing staff profile in the repository.
    /// </summary>
    void Update(StaffProfile staffProfile);

    /// <summary>
    /// Removes a staff profile from the repository.
    /// </summary>
    void Remove(StaffProfile staffProfile);
}
