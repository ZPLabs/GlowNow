namespace GlowNow.SharedKernel.Domain.Errors;

/// <summary>
/// Represents a domain error.
/// </summary>
public record Error(string Code, string Message, ErrorType Type = ErrorType.Problem)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null.", ErrorType.Problem);

    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
    public static Error Problem(string code, string message) => new(code, message, ErrorType.Problem);

    public static implicit operator string(Error error) => error.Code;
}

public enum ErrorType
{
    None = 0,
    Validation = 1,
    Problem = 2,
    NotFound = 3,
    Conflict = 4
}
