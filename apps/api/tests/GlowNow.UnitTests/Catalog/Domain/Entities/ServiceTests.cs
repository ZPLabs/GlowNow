namespace GlowNow.UnitTests.Catalog.Domain.Entities;

public class ServiceTests
{
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly Duration _duration = Duration.Create(60).Value;
    private readonly Money _price = Money.Create(50.00m).Value;
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidData()
    {
        // Act
        var result = Service.Create(
            _businessId,
            _categoryId,
            "Haircut",
            "A professional haircut",
            _duration,
            _price,
            bufferTimeMinutes: 10,
            displayOrder: 1,
            _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BusinessId.Should().Be(_businessId);
        result.Value.CategoryId.Should().Be(_categoryId);
        result.Value.Name.Should().Be("Haircut");
        result.Value.Description.Should().Be("A professional haircut");
        result.Value.Duration.Should().Be(_duration);
        result.Value.Price.Should().Be(_price);
        result.Value.BufferTimeMinutes.Should().Be(10);
        result.Value.DisplayOrder.Should().Be(1);
        result.Value.IsActive.Should().BeTrue();
        result.Value.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_RaiseServiceCreatedEvent()
    {
        // Act
        var result = Service.Create(
            _businessId,
            _categoryId,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now);

        // Assert
        result.Value.GetDomainEvents().Should().ContainSingle(e => e is ServiceCreatedEvent);
        var domainEvent = result.Value.GetDomainEvents().OfType<ServiceCreatedEvent>().First();
        domainEvent.ServiceId.Should().Be(result.Value.Id);
        domainEvent.BusinessId.Should().Be(_businessId);
        domainEvent.Name.Should().Be("Haircut");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NameIsEmpty()
    {
        // Act
        var result = Service.Create(
            _businessId,
            _categoryId,
            "",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidServiceName);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NameIsWhitespace()
    {
        // Act
        var result = Service.Create(
            _businessId,
            _categoryId,
            "   ",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidServiceName);
    }

    [Fact]
    public void Create_Should_TrimNameAndDescription()
    {
        // Act
        var result = Service.Create(
            _businessId,
            null,
            "  Haircut  ",
            "  A haircut  ",
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now);

        // Assert
        result.Value.Name.Should().Be("Haircut");
        result.Value.Description.Should().Be("A haircut");
    }

    [Fact]
    public void Create_Should_SetBufferTimeToZero_When_Negative()
    {
        // Act
        var result = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: -5,
            displayOrder: 0,
            _now);

        // Assert
        result.Value.BufferTimeMinutes.Should().Be(0);
    }

    [Fact]
    public void Create_Should_AllowNullCategoryId()
    {
        // Act
        var result = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CategoryId.Should().BeNull();
    }

    [Fact]
    public void Update_Should_ModifyProperties()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            _categoryId,
            "Haircut",
            "Old description",
            _duration,
            _price,
            bufferTimeMinutes: 10,
            displayOrder: 1,
            _now).Value;

        var newDuration = Duration.Create(90).Value;
        var newPrice = Money.Create(75.00m).Value;
        var newCategoryId = Guid.NewGuid();

        // Act
        var result = service.Update(
            newCategoryId,
            "Premium Haircut",
            "New description",
            newDuration,
            newPrice,
            bufferTimeMinutes: 15,
            displayOrder: 2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        service.CategoryId.Should().Be(newCategoryId);
        service.Name.Should().Be("Premium Haircut");
        service.Description.Should().Be("New description");
        service.Duration.Should().Be(newDuration);
        service.Price.Should().Be(newPrice);
        service.BufferTimeMinutes.Should().Be(15);
        service.DisplayOrder.Should().Be(2);
    }

    [Fact]
    public void Update_Should_RaiseServiceUpdatedEvent()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            _categoryId,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now).Value;
        service.ClearDomainEvents();

        // Act
        service.Update(null, "Updated Haircut", null, _duration, _price, 0, 0);

        // Assert
        service.GetDomainEvents().Should().ContainSingle(e => e is ServiceUpdatedEvent);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_NameIsEmpty()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now).Value;

        // Act
        var result = service.Update(null, "", null, _duration, _price, 0, 0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.InvalidServiceName);
    }

    [Fact]
    public void Activate_Should_SetIsActiveToTrue()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now).Value;
        service.Deactivate();

        // Act
        service.Activate();

        // Assert
        service.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_Should_SetIsActiveToFalse()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now).Value;

        // Act
        service.Deactivate();

        // Assert
        service.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Delete_Should_ReturnSuccess_When_ServiceIsInactive()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now).Value;
        service.Deactivate();

        // Act
        var result = service.Delete();

        // Assert
        result.IsSuccess.Should().BeTrue();
        service.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_Should_ReturnFailure_When_ServiceIsActive()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            _duration,
            _price,
            bufferTimeMinutes: 0,
            displayOrder: 0,
            _now).Value;

        // Act
        var result = service.Delete();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CatalogErrors.ServiceIsActive);
        service.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void TotalDuration_Should_IncludeBufferTime()
    {
        // Arrange
        var service = Service.Create(
            _businessId,
            null,
            "Haircut",
            null,
            Duration.Create(60).Value,
            _price,
            bufferTimeMinutes: 15,
            displayOrder: 0,
            _now).Value;

        // Act
        var totalDuration = service.TotalDuration;

        // Assert
        totalDuration.Should().Be(TimeSpan.FromMinutes(75));
    }
}
