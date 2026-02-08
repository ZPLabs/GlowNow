using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Identity.Application.Commands.Logout;

public sealed record LogoutCommand(string CognitoUserId) : ICommand;
