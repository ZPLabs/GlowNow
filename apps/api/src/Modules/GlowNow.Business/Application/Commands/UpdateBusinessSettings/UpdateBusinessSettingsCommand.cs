using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Business.Application.Commands.UpdateBusinessSettings;

/// <summary>
/// Command to update business settings.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Name">The new business name.</param>
/// <param name="Description">The new business description.</param>
/// <param name="LogoUrl">The new logo URL.</param>
public sealed record UpdateBusinessSettingsCommand(
    Guid BusinessId,
    string Name,
    string? Description,
    string? LogoUrl) : ICommand;
