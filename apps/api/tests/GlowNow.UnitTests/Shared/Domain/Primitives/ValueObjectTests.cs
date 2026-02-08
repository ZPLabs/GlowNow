namespace GlowNow.UnitTests.Shared.Domain.Primitives;

public class ValueObjectTests
{
    private sealed class TestValueObject : ValueObject
    {
        public TestValueObject(string value, int number)
        {
            Value = value;
            Number = number;
        }

        public string Value { get; }
        public int Number { get; }

        public override IEnumerable<object?> GetAtomicValues()
        {
            yield return Value;
            yield return Number;
        }
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenValuesAreEqual()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 1);
        var vo2 = new TestValueObject("test", 1);

        // Act
        var result = vo1.Equals(vo2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenValuesAreDifferent()
    {
        // Arrange
        var vo1 = new TestValueObject("test1", 1);
        var vo2 = new TestValueObject("test2", 1);

        // Act
        var result = vo1.Equals(vo2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenValuesAreEqual()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 1);
        var vo2 = new TestValueObject("test", 1);

        // Act & Assert
        (vo1 == vo2).Should().BeTrue();
    }
}
