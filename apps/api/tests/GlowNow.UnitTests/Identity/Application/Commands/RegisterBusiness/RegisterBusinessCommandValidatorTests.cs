using GlowNow.Identity.Application.Commands.RegisterBusiness;

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
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_EmailIsInvalid()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "invalid-email", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_PasswordIsTooShort()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "123", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_FirstNameIsEmpty()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_LastNameIsEmpty()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LastName");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_BusinessNameIsEmpty()
    {
        // Arrange
        var command = new RegisterBusinessCommand(
            "test@example.com", "Password123!", "John", "Doe", null,
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
            "test@example.com", "Password123!", "John", "Doe", null,
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
            "test@example.com", "Password123!", "John", "Doe", null,
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
            "test@example.com", "Password123!", "John", "Doe", null,
            "Glow Salon", "0102030405001", "Cuenca", null, null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
