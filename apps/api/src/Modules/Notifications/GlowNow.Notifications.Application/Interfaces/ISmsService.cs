namespace GlowNow.Notifications.Application.Interfaces;

/// <summary>
/// Interface for SMS sending service.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS asynchronously.
    /// </summary>
    /// <param name="to">The recipient phone number.</param>
    /// <param name="message">The SMS message content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendSmsAsync(string to, string message, CancellationToken cancellationToken = default);
}
