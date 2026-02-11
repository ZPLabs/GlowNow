using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Catalog.Application.Commands.DeleteService;

/// <summary>
/// Command to soft-delete a service.
/// </summary>
/// <param name="Id">The service ID to delete.</param>
public sealed record DeleteServiceCommand(Guid Id) : ICommand;
