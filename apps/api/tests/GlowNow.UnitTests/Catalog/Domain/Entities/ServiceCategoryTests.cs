namespace GlowNow.UnitTests.Catalog.Domain.Entities;

public class ServiceCategoryTests
{
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_Should_SetAllPropertiesCorrectly()
    {
        // Act
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            "All haircut services",
            displayOrder: 1,
            _now);

        // Assert
        category.Id.Should().NotBe(Guid.Empty);
        category.BusinessId.Should().Be(_businessId);
        category.Name.Should().Be("Haircuts");
        category.Description.Should().Be("All haircut services");
        category.DisplayOrder.Should().Be(1);
        category.CreatedAtUtc.Should().Be(_now);
        category.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_RaiseServiceCategoryCreatedEvent()
    {
        // Act
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);

        // Assert
        category.GetDomainEvents().Should().ContainSingle(e => e is ServiceCategoryCreatedEvent);
        var domainEvent = category.GetDomainEvents().OfType<ServiceCategoryCreatedEvent>().First();
        domainEvent.CategoryId.Should().Be(category.Id);
        domainEvent.BusinessId.Should().Be(_businessId);
        domainEvent.Name.Should().Be("Haircuts");
    }

    [Fact]
    public void Create_Should_TrimNameAndDescription()
    {
        // Act
        var category = ServiceCategory.Create(
            _businessId,
            "  Haircuts  ",
            "  Description  ",
            displayOrder: 0,
            _now);

        // Assert
        category.Name.Should().Be("Haircuts");
        category.Description.Should().Be("Description");
    }

    [Fact]
    public void Create_Should_AllowNullDescription()
    {
        // Act
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);

        // Assert
        category.Description.Should().BeNull();
    }

    [Fact]
    public void Update_Should_ModifyProperties()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            "Old description",
            displayOrder: 1,
            _now);

        // Act
        var result = category.Update("Updated Haircuts", "New description", displayOrder: 2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("Updated Haircuts");
        category.Description.Should().Be("New description");
        category.DisplayOrder.Should().Be(2);
    }

    [Fact]
    public void Update_Should_RaiseServiceCategoryUpdatedEvent()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);
        category.ClearDomainEvents();

        // Act
        category.Update("Updated Haircuts", null, 1);

        // Assert
        category.GetDomainEvents().Should().ContainSingle(e => e is ServiceCategoryUpdatedEvent);
        var domainEvent = category.GetDomainEvents().OfType<ServiceCategoryUpdatedEvent>().First();
        domainEvent.Name.Should().Be("Updated Haircuts");
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_NameIsEmpty()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);

        // Act
        var result = category.Update("", null, 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidCategoryName);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_NameIsWhitespace()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);

        // Act
        var result = category.Update("   ", null, 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidCategoryName);
    }

    [Fact]
    public void Update_Should_TrimNameAndDescription()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);

        // Act
        category.Update("  Updated  ", "  Description  ", 0);

        // Assert
        category.Name.Should().Be("Updated");
        category.Description.Should().Be("Description");
    }

    [Fact]
    public void Delete_Should_SetIsDeletedToTrue()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);

        // Act
        category.Delete();

        // Assert
        category.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Restore_Should_SetIsDeletedToFalse()
    {
        // Arrange
        var category = ServiceCategory.Create(
            _businessId,
            "Haircuts",
            null,
            displayOrder: 0,
            _now);
        category.Delete();

        // Act
        category.Restore();

        // Assert
        category.IsDeleted.Should().BeFalse();
    }
}
