using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Catalog.Application.Commands.CreateServiceCategory;

/// <summary>
/// Command to create a new service category.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Name">The category name.</param>
/// <param name="Description">The category description.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
public sealed record CreateServiceCategoryCommand(
    Guid BusinessId,
    string Name,
    string? Description,
    int DisplayOrder) : ICommand<CreateServiceCategoryResponse>;

/// <summary>
/// Response DTO for CreateServiceCategoryCommand.
/// </summary>
/// <param name="Id">The created category ID.</param>
public sealed record CreateServiceCategoryResponse(Guid Id);
