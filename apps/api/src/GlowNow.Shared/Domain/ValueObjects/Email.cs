using System.Text.RegularExpressions;
using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Shared.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

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

        if (!EmailRegex.IsMatch(normalizedEmail))
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
}
