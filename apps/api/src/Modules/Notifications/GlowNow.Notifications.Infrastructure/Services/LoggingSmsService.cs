using GlowNow.Notifications.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GlowNow.Notifications.Infrastructure.Services;

/// <summary>
/// A placeholder SMS service that logs messages instead of sending them.
/// Replace with actual implementation (Twilio, etc.) for production.
/// </summary>
internal sealed class LoggingSmsService : ISmsService
{
    private readonly ILogger<LoggingSmsService> _logger;

    public LoggingSmsService(ILogger<LoggingSmsService> logger)
    {
        _logger = logger;
    }

    public Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "SMS would be sent - To: {To}, Message: {Message}",
            to,
            message);

        return Task.CompletedTask;
    }
}
