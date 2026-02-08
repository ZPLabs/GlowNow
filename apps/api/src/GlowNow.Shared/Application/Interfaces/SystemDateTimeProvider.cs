namespace GlowNow.Shared.Application.Interfaces;

/// <summary>
/// Provides the current system date and time.
/// </summary>
internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
