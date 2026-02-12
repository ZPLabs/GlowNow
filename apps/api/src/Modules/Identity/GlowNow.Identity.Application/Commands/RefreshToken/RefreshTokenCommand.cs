using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Identity.Application.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponse>;
