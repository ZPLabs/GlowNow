namespace GlowNow.Team.Application.Interfaces;

/// <summary>
/// Service validator interface for cross-module service validation.
/// </summary>
public interface IServiceValidator
{
    /// <summary>
    /// Validates that a service exists and belongs to the specified business.
    /// </summary>
    /// <param name="serviceId">The service ID to validate.</param>
    /// <param name="businessId">The business ID the service should belong to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the service exists and belongs to the business, false otherwise.</returns>
    Task<bool> ValidateServiceExistsAsync(
        Guid serviceId,
        Guid businessId,
        CancellationToken cancellationToken = default);
}
