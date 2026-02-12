using GlowNow.Team.Domain.ValueObjects;

namespace GlowNow.UnitTests.Team.Domain.ValueObjects;

public class WeeklyScheduleTests
{
    [Fact]
    public void CreateEmpty_Should_HaveNoWorkDays()
    {
        // Act
        var schedule = WeeklySchedule.CreateEmpty();

        // Assert
        schedule.WorkDaysPerWeek.Should().Be(0);
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            schedule.IsWorkingOn(day).Should().BeFalse();
        }
    }

    [Fact]
    public void Create_Should_SetWorkDays()
    {
        // Arrange
        var workDay = WorkDay.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;
        var dict = new Dictionary<DayOfWeek, WorkDay?>
        {
            { DayOfWeek.Monday, workDay },
            { DayOfWeek.Tuesday, workDay }
        };

        // Act
        var result = WeeklySchedule.Create(dict);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.WorkDaysPerWeek.Should().Be(2);
        result.Value.IsWorkingOn(DayOfWeek.Monday).Should().BeTrue();
        result.Value.IsWorkingOn(DayOfWeek.Tuesday).Should().BeTrue();
        result.Value.IsWorkingOn(DayOfWeek.Wednesday).Should().BeFalse();
    }

    [Fact]
    public void IsWorkingAt_Should_ReturnCorrectValue()
    {
        // Arrange
        var workDay = WorkDay.Create(
            new TimeOnly(9, 0),
            new TimeOnly(18, 0),
            new TimeOnly(13, 0),
            new TimeOnly(14, 0)).Value;
        var dict = new Dictionary<DayOfWeek, WorkDay?>
        {
            { DayOfWeek.Monday, workDay }
        };
        var schedule = WeeklySchedule.Create(dict).Value;

        // Monday at 10:00 (Working)
        var mondayWork = new DateTime(2025, 5, 12, 10, 0, 0); // 2025-05-12 is Monday
        
        // Monday at 13:30 (Break)
        var mondayBreak = new DateTime(2025, 5, 12, 13, 30, 0);

        // Sunday (Off)
        var sunday = new DateTime(2025, 5, 11, 10, 0, 0);

        // Assert
        schedule.IsWorkingAt(mondayWork).Should().BeTrue();
        schedule.IsWorkingAt(mondayBreak).Should().BeFalse();
        schedule.IsWorkingAt(sunday).Should().BeFalse();
    }

    [Fact]
    public void ToJson_And_FromJson_Should_BeConsistent()
    {
        // Arrange
        var workDay = WorkDay.Create(
            new TimeOnly(9, 0), 
            new TimeOnly(18, 0), 
            new TimeOnly(13, 0), 
            new TimeOnly(14, 0)).Value;
        var dict = new Dictionary<DayOfWeek, WorkDay?>
        {
            { DayOfWeek.Monday, workDay },
            { DayOfWeek.Wednesday, workDay }
        };
        var original = WeeklySchedule.Create(dict).Value;

        // Act
        var json = original.ToJson();
        var result = WeeklySchedule.FromJson(json);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.WorkDaysPerWeek.Should().Be(original.WorkDaysPerWeek);
        result.Value.IsWorkingOn(DayOfWeek.Monday).Should().BeTrue();
        result.Value.GetWorkDayFor(DayOfWeek.Monday)!.StartTime.Should().Be(workDay.StartTime);
        result.Value.GetWorkDayFor(DayOfWeek.Monday)!.BreakStart.Should().Be(workDay.BreakStart);
    }
}
