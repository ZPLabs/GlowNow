namespace GlowNow.UnitTests.Catalog.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_AmountIsValid()
    {
        // Act
        var result = Money.Create(49.99m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(49.99m);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_AmountIsZero()
    {
        // Act
        var result = Money.Create(0m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(0m);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_AmountIsMaximum()
    {
        // Act
        var result = Money.Create(Money.MaxAmount);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(999999.99m);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_AmountIsNegative()
    {
        // Act
        var result = Money.Create(-1m);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidPrice);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_AmountExceedsMaximum()
    {
        // Act
        var result = Money.Create(Money.MaxAmount + 0.01m);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidPrice);
    }

    [Fact]
    public void Create_Should_RoundToTwoDecimalPlaces()
    {
        // Act
        var result = Money.Create(49.999m);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(50.00m);
    }

    [Fact]
    public void Zero_Should_ReturnMoneyWithZeroAmount()
    {
        // Act
        var zero = Money.Zero;

        // Assert
        zero.Amount.Should().Be(0m);
    }

    [Fact]
    public void Addition_Should_ReturnCorrectSum()
    {
        // Arrange
        var money1 = Money.Create(10.50m).Value;
        var money2 = Money.Create(5.25m).Value;

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(15.75m);
    }

    [Fact]
    public void Subtraction_Should_ReturnCorrectDifference()
    {
        // Arrange
        var money1 = Money.Create(10.50m).Value;
        var money2 = Money.Create(5.25m).Value;

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(5.25m);
    }

    [Fact]
    public void Subtraction_Should_ReturnZero_When_ResultWouldBeNegative()
    {
        // Arrange
        var money1 = Money.Create(5m).Value;
        var money2 = Money.Create(10m).Value;

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(0m);
    }

    [Fact]
    public void Multiplication_Should_ReturnCorrectProduct()
    {
        // Arrange
        var money = Money.Create(10.00m).Value;

        // Act
        var result = money * 2.5m;

        // Assert
        result.Amount.Should().Be(25.00m);
    }

    [Fact]
    public void Multiplication_Should_RoundToTwoDecimalPlaces()
    {
        // Arrange
        var money = Money.Create(10.00m).Value;

        // Act
        var result = money * 0.333m;

        // Assert
        result.Amount.Should().Be(3.33m);
    }

    [Fact]
    public void ImplicitConversion_Should_ReturnAmount()
    {
        // Arrange
        var money = Money.Create(49.99m).Value;

        // Act
        decimal amount = money;

        // Assert
        amount.Should().Be(49.99m);
    }

    [Fact]
    public void ToString_Should_ReturnFormattedString()
    {
        // Arrange
        var money = Money.Create(49.99m).Value;

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("$49.99 USD");
    }

    [Fact]
    public void Currency_Should_BeUSD()
    {
        // Assert
        Money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_When_ValuesAreEqual()
    {
        // Arrange
        var money1 = Money.Create(49.99m).Value;
        var money2 = Money.Create(49.99m).Value;

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ValuesAreDifferent()
    {
        // Arrange
        var money1 = Money.Create(49.99m).Value;
        var money2 = Money.Create(50.00m).Value;

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
    }
}
