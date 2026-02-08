using System.Text.RegularExpressions;
using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Business.Domain.ValueObjects;

public sealed class Ruc : ValueObject
{
    private static readonly Regex RucRegex = new(@"^[0-9]{10}([0-9]{3})?$", RegexOptions.Compiled);

    private Ruc(string value) => Value = value;

    public string Value { get; }

    public static Result<Ruc> Create(string? ruc)
    {
        if (string.IsNullOrWhiteSpace(ruc))
        {
            return Result.Failure<Ruc>(Error.Validation("Ruc.Empty", "RUC/Cédula cannot be empty."));
        }

        if (!RucRegex.IsMatch(ruc))
        {
            return Result.Failure<Ruc>(Error.Validation("Ruc.InvalidFormat", "RUC must be 13 digits and Cédula must be 10 digits."));
        }

        // Basic province code validation (01-24 or 30 for foreigners)
        int provinceCode = int.Parse(ruc.Substring(0, 2));
        if (!((provinceCode >= 1 && provinceCode <= 24) || provinceCode == 30))
        {
            return Result.Failure<Ruc>(Error.Validation("Ruc.InvalidProvince", "Invalid province code in RUC/Cédula."));
        }

        return new Ruc(ruc);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(Ruc ruc) => ruc.Value;
}
