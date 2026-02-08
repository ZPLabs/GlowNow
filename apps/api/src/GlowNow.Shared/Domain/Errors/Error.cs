namespace GlowNow.Shared.Domain.Errors;

/// <summary>
/// Represents a domain error.
/// </summary>
public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null.");

    public static Error Validation(string code, string message) => new(code, message);

    public static implicit operator string(Error error) => error.Code;
}
