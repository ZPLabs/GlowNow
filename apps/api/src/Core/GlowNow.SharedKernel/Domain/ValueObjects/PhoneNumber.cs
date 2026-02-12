using System.Text.RegularExpressions;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.SharedKernel.Domain.ValueObjects;

public sealed partial class PhoneNumber : ValueObject
{
    private PhoneNumber(string value) => Value = value;

    public string Value { get; }

    public static Result<PhoneNumber> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result.Failure<PhoneNumber>(Error.Validation("PhoneNumber.Empty", "Phone number cannot be empty."));
        }

        string normalizedPhoneNumber = StripFormatting().Replace(phoneNumber, "");

        if (!normalizedPhoneNumber.StartsWith('+'))
        {
            // If it doesn't start with +, assume it might be local and try to fix it or just fail
            // For MVP, we enforce +593
            if (normalizedPhoneNumber.StartsWith("09"))
            {
                normalizedPhoneNumber = "+593" + normalizedPhoneNumber[1..];
            }
            else if (normalizedPhoneNumber.StartsWith('9'))
            {
                normalizedPhoneNumber = "+593" + normalizedPhoneNumber;
            }
        }

        if (!PhoneNumberRegex().IsMatch(normalizedPhoneNumber))
        {
            return Result.Failure<PhoneNumber>(Error.Validation("PhoneNumber.InvalidFormat", "Phone number must be in Ecuador format (+593 followed by 9 digits)."));
        }

        return new PhoneNumber(normalizedPhoneNumber);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

    [GeneratedRegex(@"^\+593[0-9]{9}$", RegexOptions.Compiled)]
    private static partial Regex PhoneNumberRegex();

    [GeneratedRegex(@"[\s\-\(\)]", RegexOptions.Compiled)]
    private static partial Regex StripFormatting();
}
