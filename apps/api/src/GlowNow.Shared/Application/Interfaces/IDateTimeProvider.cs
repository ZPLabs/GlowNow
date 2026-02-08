namespace GlowNow.Shared.Application.Interfaces;

/// <summary>
/// Defines a contract for providing the current date and time.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
