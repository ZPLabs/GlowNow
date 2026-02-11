using GlowNow.Catalog.Domain.Errors;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Catalog.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount in USD (used in Ecuador).
/// </summary>
public sealed class Money : ValueObject
{
    public const string Currency = "USD";
    public const decimal MinAmount = 0m;
    public const decimal MaxAmount = 999999.99m;

    private Money(decimal amount)
    {
        Amount = amount;
    }

    private Money()
    {
        Amount = 0m;
    }

    /// <summary>
    /// Gets the amount in USD.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Creates a new Money instance.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <returns>A Result containing the Money or an error.</returns>
    public static Result<Money> Create(decimal amount)
    {
        if (amount < MinAmount)
        {
            return Result.Failure<Money>(CatalogErrors.InvalidPrice);
        }

        if (amount > MaxAmount)
        {
            return Result.Failure<Money>(CatalogErrors.InvalidPrice);
        }

        // Round to 2 decimal places
        var roundedAmount = Math.Round(amount, 2);
        return new Money(roundedAmount);
    }

    /// <summary>
    /// Creates a zero money instance.
    /// </summary>
    public static Money Zero => new(0m);

    /// <summary>
    /// Adds two money amounts.
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        return new Money(left.Amount + right.Amount);
    }

    /// <summary>
    /// Subtracts two money amounts.
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        var result = left.Amount - right.Amount;
        return new Money(result < 0 ? 0 : result);
    }

    /// <summary>
    /// Multiplies money by a factor.
    /// </summary>
    public static Money operator *(Money money, decimal factor)
    {
        return new Money(Math.Round(money.Amount * factor, 2));
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return Amount;
    }

    public override string ToString() => $"${Amount:N2} {Currency}";

    public static implicit operator decimal(Money money) => money.Amount;
}
