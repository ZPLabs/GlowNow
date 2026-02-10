using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Catalog.Application.Commands.UpdateServiceCategory;

/// <summary>
/// Command to update an existing service category.
/// </summary>
/// <param name="Id">The category ID.</param>
/// <param name="Name">The new category name.</param>
/// <param name="Description">The new category description.</param>
/// <param name="DisplayOrder">The new display order for sorting.</param>
public sealed record UpdateServiceCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder) : ICommand;
