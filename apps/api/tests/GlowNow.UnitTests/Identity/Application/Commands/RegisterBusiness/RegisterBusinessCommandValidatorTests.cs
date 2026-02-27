using GlowNow.Identity.Application.Commands.RegisterBusiness;
using FluentAssertions;
using Xunit;

namespace GlowNow.UnitTests.Identity.Application.Commands.RegisterBusiness;

public class RegisterBusinessCommandValidatorTests
{
    private readonly RegisterBusinessCommandValidator _validator;

    public RegisterBusinessCommandValidatorTests()
    {
        _validator = new RegisterBusinessCommandValidator();
    }

    [Fact]
    public void Validate_Should_ReturnSuccess_When_CommandIsValid()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_BusinessNameIsEmpty()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BusinessName");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_BusinessRucIsEmpty()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BusinessRuc");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_BusinessAddressIsEmpty()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BusinessAddress");
    }

    [Fact]
    public void Validate_Should_ReturnSuccess_When_OptionalPhoneFieldsAreNull()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
