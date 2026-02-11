using GlowNow.Notifications.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GlowNow.Notifications.Infrastructure.Services;

/// <summary>
/// A placeholder email service that logs emails instead of sending them.
/// Replace with actual implementation (SendGrid, etc.) for production.
/// </summary>
internal sealed class LoggingEmailService : IEmailService
{
    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email would be sent - To: {To}, Subject: {Subject}, Body length: {BodyLength}",
            to,
            subject,
            body?.Length ?? 0);

        return Task.CompletedTask;
    }
}
