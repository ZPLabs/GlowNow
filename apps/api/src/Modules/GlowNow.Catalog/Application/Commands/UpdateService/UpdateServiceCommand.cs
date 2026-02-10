using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Catalog.Application.Commands.UpdateService;

/// <summary>
/// Command to update an existing service.
/// </summary>
/// <param name="Id">The service ID.</param>
/// <param name="CategoryId">The optional category ID.</param>
/// <param name="Name">The new service name.</param>
/// <param name="Description">The new service description.</param>
/// <param name="DurationMinutes">The new duration in minutes.</param>
/// <param name="Price">The new price in USD.</param>
/// <param name="BufferTimeMinutes">The new buffer time after service in minutes.</param>
/// <param name="DisplayOrder">The new display order for sorting.</param>
public sealed record UpdateServiceCommand(
    Guid Id,
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder) : ICommand;
