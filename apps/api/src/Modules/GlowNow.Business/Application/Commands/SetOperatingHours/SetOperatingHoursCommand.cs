using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Business.Application.Commands.SetOperatingHours;

/// <summary>
/// Command to set the operating hours for a business.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Schedule">A dictionary mapping day of week to operating hours (null means closed).</param>
public sealed record SetOperatingHoursCommand(
    Guid BusinessId,
    Dictionary<DayOfWeek, DayScheduleDto?> Schedule) : ICommand;

/// <summary>
/// Represents the operating hours for a single day.
/// </summary>
/// <param name="OpenTime">Opening time in HH:mm format.</param>
/// <param name="CloseTime">Closing time in HH:mm format.</param>
public sealed record DayScheduleDto(string OpenTime, string CloseTime);
