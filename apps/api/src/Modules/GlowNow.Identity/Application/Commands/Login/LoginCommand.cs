using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Identity.Application.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
