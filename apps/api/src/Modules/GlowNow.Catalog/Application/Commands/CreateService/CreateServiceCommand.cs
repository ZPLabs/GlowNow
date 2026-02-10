using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Catalog.Application.Commands.CreateService;

/// <summary>
/// Command to create a new service.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="CategoryId">The optional category ID.</param>
/// <param name="Name">The service name.</param>
/// <param name="Description">The service description.</param>
/// <param name="DurationMinutes">The duration in minutes.</param>
/// <param name="Price">The price in USD.</param>
/// <param name="BufferTimeMinutes">The buffer time after service in minutes.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
public sealed record CreateServiceCommand(
    Guid BusinessId,
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder) : ICommand<CreateServiceResponse>;

/// <summary>
/// Response DTO for CreateServiceCommand.
/// </summary>
/// <param name="Id">The created service ID.</param>
public sealed record CreateServiceResponse(Guid Id);
