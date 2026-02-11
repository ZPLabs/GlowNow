namespace GlowNow.Infrastructure.Core.Application.Interfaces;

/// <summary>
/// Defines a contract for providing the current date and time.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
