namespace GlowNow.UnitTests.Shared.Domain.Primitives;

public class EntityTests
{
    private sealed class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenIdsAreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenIdsAreDifferent()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act
        var result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EqualityOperator_ShouldReturnTrue_WhenIdsAreEqual()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act & Assert
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void InequalityOperator_ShouldReturnTrue_WhenIdsAreDifferent()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act & Assert
        (entity1 != entity2).Should().BeTrue();
    }
}
