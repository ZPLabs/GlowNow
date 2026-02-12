namespace GlowNow.Business.Application.Queries.GetBusinessDetails;

/// <summary>
/// Response DTO containing full business details.
/// </summary>
/// <param name="Id">The business ID.</param>
/// <param name="Name">The business name.</param>
/// <param name="Ruc">The business RUC.</param>
/// <param name="Address">The business address.</param>
/// <param name="PhoneNumber">The business phone number.</param>
/// <param name="Email">The business email.</param>
/// <param name="Description">The business description.</param>
/// <param name="LogoUrl">The business logo URL.</param>
/// <param name="OperatingHours">The weekly operating hours.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the business was created.</param>
public sealed record BusinessDetailsResponse(
    Guid Id,
    string Name,
    string Ruc,
    string Address,
    string? PhoneNumber,
    string Email,
    string? Description,
    string? LogoUrl,
    OperatingHoursResponse OperatingHours,
    DateTime CreatedAtUtc);

/// <summary>
/// Response DTO for operating hours.
/// </summary>
/// <param name="Schedule">A dictionary mapping day of week to time range (null means closed).</param>
public sealed record OperatingHoursResponse(Dictionary<DayOfWeek, TimeRangeResponse?> Schedule);

/// <summary>
/// Response DTO for a time range.
/// </summary>
/// <param name="OpenTime">Opening time in HH:mm format.</param>
/// <param name="CloseTime">Closing time in HH:mm format.</param>
public sealed record TimeRangeResponse(string OpenTime, string CloseTime);
