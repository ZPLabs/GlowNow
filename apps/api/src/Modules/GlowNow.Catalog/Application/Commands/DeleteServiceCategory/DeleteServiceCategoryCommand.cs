using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Catalog.Application.Commands.DeleteServiceCategory;

/// <summary>
/// Command to soft-delete a service category.
/// </summary>
/// <param name="Id">The category ID to delete.</param>
public sealed record DeleteServiceCategoryCommand(Guid Id) : ICommand;
