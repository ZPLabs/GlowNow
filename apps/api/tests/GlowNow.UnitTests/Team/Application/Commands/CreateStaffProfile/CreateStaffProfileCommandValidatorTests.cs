using FluentValidation.TestHelper;
using GlowNow.Team.Application.Commands.CreateStaffProfile;

namespace GlowNow.UnitTests.Team.Application.Commands.CreateStaffProfile;

public class CreateStaffProfileCommandValidatorTests
{
    private readonly CreateStaffProfileCommandValidator _validator;

    public CreateStaffProfileCommandValidatorTests()
    {
        _validator = new CreateStaffProfileCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_BusinessIdIsEmpty()
    {
        var command = new CreateStaffProfileCommand(Guid.Empty, Guid.NewGuid(), "Title", null, null, 0, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.BusinessId);
    }

    [Fact]
    public void Should_HaveError_When_UserIdIsEmpty()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.Empty, "Title", null, null, 0, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_HaveError_When_TitleIsEmpty()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.NewGuid(), "", null, null, 0, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_HaveError_When_TitleExceedsMaxLength()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.NewGuid(), new string('a', 101), null, null, 0, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_HaveError_When_BioExceedsMaxLength()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", new string('a', 1001), null, 0, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Bio);
    }

    [Fact]
    public void Should_HaveError_When_ProfileImageUrlExceedsMaxLength()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", null, new string('a', 501), 0, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ProfileImageUrl);
    }

    [Fact]
    public void Should_HaveError_When_DisplayOrderIsNegative()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", null, null, -1, true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        var command = new CreateStaffProfileCommand(Guid.NewGuid(), Guid.NewGuid(), "Title", "Bio", "https://img.com", 1, true);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
