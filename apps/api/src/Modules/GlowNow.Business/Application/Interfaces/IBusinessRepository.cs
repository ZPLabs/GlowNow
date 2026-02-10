using GlowNow.Business.Domain.ValueObjects;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Application.Interfaces;

/// <summary>
/// Repository interface for Business aggregate operations.
/// </summary>
public interface IBusinessRepository
{
    /// <summary>
    /// Gets a business by its unique identifier.
    /// </summary>
    /// <param name="id">The business ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The business if found, null otherwise.</returns>
    Task<BusinessEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a business with the given RUC exists.
    /// </summary>
    /// <param name="ruc">The RUC to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a business with the RUC exists, false otherwise.</returns>
    Task<bool> ExistsByRucAsync(Ruc ruc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new business to the repository.
    /// </summary>
    /// <param name="business">The business to add.</param>
    void Add(BusinessEntity business);

    /// <summary>
    /// Updates an existing business in the repository.
    /// </summary>
    /// <param name="business">The business to update.</param>
    void Update(BusinessEntity business);
}
