namespace GlowNow.UnitTests.Shared.Domain.Primitives;

public class AggregateRootTests
{
    private sealed class TestDomainEvent : IDomainEvent;

    private sealed class TestAggregateRoot : AggregateRoot<Guid>
    {
        public TestAggregateRoot(Guid id) : base(id) { }
        
        public void AddTestEvent() => RaiseDomainEvent(new TestDomainEvent());
    }

    [Fact]
    public void GetDomainEvents_ShouldReturnAddedEvents()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid());

        // Act
        aggregate.AddTestEvent();

        // Assert
        aggregate.GetDomainEvents().Should().ContainSingle()
            .Which.Should().BeOfType<TestDomainEvent>();
    }

    [Fact]
    public void ClearDomainEvents_ShouldEmptyEventsList()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(Guid.NewGuid());
        aggregate.AddTestEvent();

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        aggregate.GetDomainEvents().Should().BeEmpty();
    }
}
