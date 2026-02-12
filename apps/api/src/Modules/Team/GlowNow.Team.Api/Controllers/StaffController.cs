using GlowNow.Team.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using GlowNow.Team.Application.Commands.ActivateStaff;
using GlowNow.Team.Application.Commands.ApproveTimeOff;
using GlowNow.Team.Application.Commands.AssignServiceToStaff;
using GlowNow.Team.Application.Commands.CancelTimeOff;
using GlowNow.Team.Application.Commands.CreateStaffProfile;
using GlowNow.Team.Application.Commands.DeactivateStaff;
using GlowNow.Team.Application.Commands.DeleteStaffProfile;
using GlowNow.Team.Application.Commands.RejectTimeOff;
using GlowNow.Team.Application.Commands.RequestTimeOff;
using GlowNow.Team.Application.Commands.UnassignServiceFromStaff;
using GlowNow.Team.Application.Commands.UpdateStaffProfile;
using GlowNow.Team.Application.Commands.UpdateStaffSchedule;
using GlowNow.Team.Application.Queries.GetAllStaff;
using GlowNow.Team.Application.Queries.GetStaffByService;
using GlowNow.Team.Application.Queries.GetStaffProfile;
using GlowNow.Team.Application.Queries.GetStaffSchedule;
using GlowNow.Team.Application.Queries.GetStaffTimeOff;
using GlowNow.Team.Application.Queries.GetStaffBlockedTimes;
using GlowNow.Team.Application.Queries.GetStaffAvailability;
using GlowNow.Team.Application.Queries.GetMySchedule;
using GlowNow.Team.Application.Commands.CreateBlockedTime;
using GlowNow.Team.Application.Commands.DeleteBlockedTime;
using GlowNow.Team.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlowNow.Team.Api.Controllers;

/// <summary>
/// API controller for staff management.
/// </summary>
[ApiController]
[Route("api/v1/staff")]
[Produces("application/json")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly ISender _sender;

    public StaffController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create staff profile.
    /// </summary>
    /// <remarks>
    /// Creates a new staff profile for an existing user within a business. The user must be a member of the business. Each user can only have one staff profile per business.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateStaffProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStaffProfile([FromBody] CreateStaffRequest request)
    {
        var command = new CreateStaffProfileCommand(
            request.BusinessId,
            request.UserId,
            request.Title,
            request.Bio,
            request.ProfileImageUrl,
            request.DisplayOrder,
            request.AcceptsOnlineBookings);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// List staff.
    /// </summary>
    /// <remarks>
    /// Retrieves all staff profiles for a business, ordered by display order and then by title. Use activeOnly=true to filter to only active staff members.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StaffProfileResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllStaff([FromQuery] Guid businessId, [FromQuery] bool? activeOnly)
    {
        var result = await _sender.Send(new GetAllStaffQuery(businessId, activeOnly ?? false));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get staff profile.
    /// </summary>
    /// <remarks>
    /// Retrieves the full details of a specific staff profile including title, bio, schedule settings, and assigned services.
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StaffProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffProfile(Guid id)
    {
        var result = await _sender.Send(new GetStaffProfileQuery(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Update staff profile.
    /// </summary>
    /// <remarks>
    /// Updates an existing staff profile's details including title, bio, profile image, and booking preferences.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStaffProfile(Guid id, [FromBody] UpdateStaffRequest request)
    {
        var command = new UpdateStaffProfileCommand(
            id,
            request.Title,
            request.Bio,
            request.ProfileImageUrl,
            request.DisplayOrder,
            request.AcceptsOnlineBookings);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get staff schedule.
    /// </summary>
    /// <remarks>
    /// Retrieves the default weekly working schedule for a staff member, including work hours and break times for each day of the week.
    /// </remarks>
    [HttpGet("{id:guid}/schedule")]
    [ProducesResponseType(typeof(ApiResponse<StaffScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffSchedule(Guid id)
    {
        var result = await _sender.Send(new GetStaffScheduleQuery(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Update staff schedule.
    /// </summary>
    /// <remarks>
    /// Updates the default weekly working schedule for a staff member. Set a day to null to mark it as a day off. Times should be in HH:mm format.
    /// </remarks>
    [HttpPut("{id:guid}/schedule")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStaffSchedule(Guid id, [FromBody] UpdateStaffScheduleRequest request)
    {
        var scheduleDict = request.Schedule.ToDictionary(
            kvp => Enum.Parse<DayOfWeek>(kvp.Key),
            kvp => kvp.Value is null ? null : new WorkDayInput(
                kvp.Value.StartTime,
                kvp.Value.EndTime,
                kvp.Value.BreakStart,
                kvp.Value.BreakEnd));

        var command = new UpdateStaffScheduleCommand(id, scheduleDict);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Activate staff.
    /// </summary>
    /// <remarks>
    /// Activates a staff member, allowing them to receive bookings. Staff must be activated before they can be assigned to appointments.
    /// </remarks>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ActivateStaff(Guid id)
    {
        var result = await _sender.Send(new ActivateStaffCommand(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Deactivate staff.
    /// </summary>
    /// <remarks>
    /// Deactivates a staff member, preventing them from receiving new bookings. Existing appointments are not affected.
    /// </remarks>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivateStaff(Guid id)
    {
        var result = await _sender.Send(new DeactivateStaffCommand(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete staff profile.
    /// </summary>
    /// <remarks>
    /// Soft-deletes a staff profile. The staff member must be deactivated first. Historical data is preserved.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteStaffProfile(Guid id)
    {
        var result = await _sender.Send(new DeleteStaffProfileCommand(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Assign service to staff.
    /// </summary>
    /// <remarks>
    /// Assigns a service to a staff member, allowing them to perform that service. The service must belong to the same business as the staff member.
    /// </remarks>
    [HttpPost("{id:guid}/services")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignServiceToStaff(Guid id, [FromBody] AssignServiceRequest request)
    {
        var command = new AssignServiceToStaffCommand(id, request.ServiceId);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Unassign service from staff.
    /// </summary>
    /// <remarks>
    /// Removes a service assignment from a staff member. The staff member will no longer be able to perform this service.
    /// </remarks>
    [HttpDelete("{id:guid}/services/{serviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignServiceFromStaff(Guid id, Guid serviceId)
    {
        var command = new UnassignServiceFromStaffCommand(id, serviceId);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Get staff by service.
    /// </summary>
    /// <remarks>
    /// Retrieves all active staff members who can perform a specific service. Useful for showing available staff during booking.
    /// </remarks>
    [HttpGet("by-service/{serviceId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<StaffProfileResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStaffByService(Guid serviceId)
    {
        var result = await _sender.Send(new GetStaffByServiceQuery(serviceId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get staff time off.
    /// </summary>
    /// <remarks>
    /// Retrieves all time off requests for a staff member. Optionally filter by date range.
    /// </remarks>
    [HttpGet("{id:guid}/time-off")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TimeOffResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffTimeOff(Guid id, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
    {
        var result = await _sender.Send(new GetStaffTimeOffQuery(id, startDate, endDate));
        return result.ToActionResult();
    }

    /// <summary>
    /// Request time off.
    /// </summary>
    /// <remarks>
    /// Creates a new time off request for a staff member. The request starts in Pending status and must be approved by a manager.
    /// </remarks>
    [HttpPost("{id:guid}/time-off")]
    [ProducesResponseType(typeof(ApiResponse<RequestTimeOffResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RequestTimeOff(Guid id, [FromBody] RequestTimeOffRequest request)
    {
        var command = new RequestTimeOffCommand(
            id,
            request.StartDate,
            request.EndDate,
            request.Type,
            request.Notes);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Approve time off.
    /// </summary>
    /// <remarks>
    /// Approves a pending time off request. Only managers can approve time off requests.
    /// </remarks>
    [HttpPost("{id:guid}/time-off/{timeOffId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveTimeOff(Guid id, Guid timeOffId, [FromBody] ApproveTimeOffRequest request)
    {
        var command = new ApproveTimeOffCommand(timeOffId, request.ApprovedByUserId);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Reject time off.
    /// </summary>
    /// <remarks>
    /// Rejects a pending time off request with an optional reason.
    /// </remarks>
    [HttpPost("{id:guid}/time-off/{timeOffId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RejectTimeOff(Guid id, Guid timeOffId, [FromBody] RejectTimeOffRequest request)
    {
        var command = new RejectTimeOffCommand(timeOffId, request.Reason);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Cancel time off.
    /// </summary>
    /// <remarks>
    /// Cancels a time off request. Can only be cancelled if it hasn't started yet.
    /// </remarks>
    [HttpDelete("{id:guid}/time-off/{timeOffId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelTimeOff(Guid id, Guid timeOffId)
    {
        var result = await _sender.Send(new CancelTimeOffCommand(timeOffId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get staff blocked times.
    /// </summary>
    /// <remarks>
    /// Retrieves all blocked times for a staff member. Includes both recurring and one-time blocks. Optionally filter by date range.
    /// </remarks>
    [HttpGet("{id:guid}/blocked-times")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BlockedTimeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffBlockedTimes(Guid id, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
    {
        var result = await _sender.Send(new GetStaffBlockedTimesQuery(id, startDate, endDate));
        return result.ToActionResult();
    }

    /// <summary>
    /// Create blocked time.
    /// </summary>
    /// <remarks>
    /// Creates a new blocked time for a staff member. Can be recurring (repeats weekly) or one-time (specific date).
    /// </remarks>
    [HttpPost("{id:guid}/blocked-times")]
    [ProducesResponseType(typeof(ApiResponse<CreateBlockedTimeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBlockedTime(Guid id, [FromBody] CreateBlockedTimeRequest request)
    {
        var command = new CreateBlockedTimeCommand(
            id,
            request.Title,
            request.StartTime,
            request.EndTime,
            request.IsRecurring,
            request.DayOfWeek,
            request.SpecificDate);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete blocked time.
    /// </summary>
    /// <remarks>
    /// Deletes a blocked time from a staff member's schedule.
    /// </remarks>
    [HttpDelete("{id:guid}/blocked-times/{blockedTimeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBlockedTime(Guid id, Guid blockedTimeId)
    {
        var result = await _sender.Send(new DeleteBlockedTimeCommand(blockedTimeId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get staff availability.
    /// </summary>
    /// <remarks>
    /// Calculates the available time slots for a staff member within a date range. Takes into account the default schedule, approved time off, and blocked times.
    /// </remarks>
    [HttpGet("{id:guid}/availability")]
    [ProducesResponseType(typeof(ApiResponse<StaffAvailabilityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStaffAvailability(Guid id, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
    {
        var result = await _sender.Send(new GetStaffAvailabilityQuery(id, startDate, endDate));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get my schedule.
    /// </summary>
    /// <remarks>
    /// Retrieves the current user's weekly schedule. The user must have a staff profile in the specified business.
    /// </remarks>
    [HttpGet("my-schedule")]
    [ProducesResponseType(typeof(ApiResponse<StaffScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMySchedule([FromQuery] Guid userId, [FromQuery] Guid businessId)
    {
        var result = await _sender.Send(new GetMyScheduleQuery(userId, businessId));
        return result.ToActionResult();
    }
}

#region Request DTOs

/// <summary>
/// Request DTO for creating a staff profile.
/// </summary>
public sealed record CreateStaffRequest(
    Guid BusinessId,
    Guid UserId,
    string Title,
    string? Bio,
    string? ProfileImageUrl,
    int DisplayOrder,
    bool AcceptsOnlineBookings);

/// <summary>
/// Request DTO for updating a staff profile.
/// </summary>
public sealed record UpdateStaffRequest(
    string Title,
    string? Bio,
    string? ProfileImageUrl,
    int DisplayOrder,
    bool AcceptsOnlineBookings);

/// <summary>
/// Request DTO for updating a staff schedule.
/// </summary>
public sealed record UpdateStaffScheduleRequest(
    Dictionary<string, WorkDayInputDto?> Schedule);

/// <summary>
/// Input DTO for a single work day in the schedule.
/// </summary>
public sealed record WorkDayInputDto(
    string StartTime,
    string EndTime,
    string? BreakStart,
    string? BreakEnd);

/// <summary>
/// Request DTO for assigning a service to a staff member.
/// </summary>
public sealed record AssignServiceRequest(Guid ServiceId);

/// <summary>
/// Request DTO for requesting time off.
/// </summary>
public sealed record RequestTimeOffRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOffType Type,
    string? Notes);

/// <summary>
/// Request DTO for approving time off.
/// </summary>
public sealed record ApproveTimeOffRequest(Guid ApprovedByUserId);

/// <summary>
/// Request DTO for rejecting time off.
/// </summary>
public sealed record RejectTimeOffRequest(string? Reason);

/// <summary>
/// Request DTO for creating a blocked time.
/// </summary>
public sealed record CreateBlockedTimeRequest(
    string? Title,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsRecurring,
    DayOfWeek? DayOfWeek,
    DateOnly? SpecificDate);

#endregion
