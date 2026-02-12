using System.Text.RegularExpressions;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.SharedKernel.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    public const int MaxLength = 256;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Result<Email> Create(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<Email>(Error.Validation("Email.Empty", "Email cannot be empty."));
        }

        string normalizedEmail = email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > MaxLength)
        {
            return Result.Failure<Email>(Error.Validation("Email.TooLong", $"Email cannot exceed {MaxLength} characters."));
        }

        if (!EmailRegex().IsMatch(normalizedEmail))
        {
            return Result.Failure<Email>(Error.Validation("Email.InvalidFormat", "Email format is invalid."));
        }

        return new Email(normalizedEmail);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(Email email) => email.Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}
