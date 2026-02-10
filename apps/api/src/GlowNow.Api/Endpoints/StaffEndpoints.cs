using GlowNow.Api.Infrastructure;
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

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for staff management.
/// </summary>
public static class StaffEndpoints
{
    public static void MapStaffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/staff")
            .WithTags("Staff")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/", async (CreateStaffRequest request, ISender sender) =>
        {
            var command = new CreateStaffProfileCommand(
                request.BusinessId,
                request.UserId,
                request.Title,
                request.Bio,
                request.ProfileImageUrl,
                request.DisplayOrder,
                request.AcceptsOnlineBookings);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("CreateStaffProfile")
        .WithSummary("Create staff profile")
        .WithDescription("Creates a new staff profile for an existing user within a business. The user must be a member of the business. Each user can only have one staff profile per business.")
        .Produces<ApiResponse<CreateStaffProfileResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", async (Guid businessId, bool? activeOnly, ISender sender) =>
        {
            var result = await sender.Send(new GetAllStaffQuery(businessId, activeOnly ?? false));
            return result.ToApiResponse();
        })
        .WithName("GetAllStaff")
        .WithSummary("List staff")
        .WithDescription("Retrieves all staff profiles for a business, ordered by display order and then by title. Use activeOnly=true to filter to only active staff members.")
        .Produces<ApiResponse<IReadOnlyList<StaffProfileResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffProfileQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetStaffProfile")
        .WithSummary("Get staff profile")
        .WithDescription("Retrieves the full details of a specific staff profile including title, bio, schedule settings, and assigned services.")
        .Produces<ApiResponse<StaffProfileResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (Guid id, UpdateStaffRequest request, ISender sender) =>
        {
            var command = new UpdateStaffProfileCommand(
                id,
                request.Title,
                request.Bio,
                request.ProfileImageUrl,
                request.DisplayOrder,
                request.AcceptsOnlineBookings);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateStaffProfile")
        .WithSummary("Update staff profile")
        .WithDescription("Updates an existing staff profile's details including title, bio, profile image, and booking preferences.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/schedule", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffScheduleQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetStaffSchedule")
        .WithSummary("Get staff schedule")
        .WithDescription("Retrieves the default weekly working schedule for a staff member, including work hours and break times for each day of the week.")
        .Produces<ApiResponse<StaffScheduleResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/schedule", async (Guid id, UpdateStaffScheduleRequest request, ISender sender) =>
        {
            var scheduleDict = request.Schedule.ToDictionary(
                kvp => Enum.Parse<DayOfWeek>(kvp.Key),
                kvp => kvp.Value is null ? null : new WorkDayInput(
                    kvp.Value.StartTime,
                    kvp.Value.EndTime,
                    kvp.Value.BreakStart,
                    kvp.Value.BreakEnd));

            var command = new UpdateStaffScheduleCommand(id, scheduleDict);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateStaffSchedule")
        .WithSummary("Update staff schedule")
        .WithDescription("Updates the default weekly working schedule for a staff member. Set a day to null to mark it as a day off. Times should be in HH:mm format.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/activate", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ActivateStaffCommand(id));
            return result.ToApiResponse();
        })
        .WithName("ActivateStaff")
        .WithSummary("Activate staff")
        .WithDescription("Activates a staff member, allowing them to receive bookings. Staff must be activated before they can be assigned to appointments.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/deactivate", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeactivateStaffCommand(id));
            return result.ToApiResponse();
        })
        .WithName("DeactivateStaff")
        .WithSummary("Deactivate staff")
        .WithDescription("Deactivates a staff member, preventing them from receiving new bookings. Existing appointments are not affected.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteStaffProfileCommand(id));
            return result.ToApiResponse();
        })
        .WithName("DeleteStaffProfile")
        .WithSummary("Delete staff profile")
        .WithDescription("Soft-deletes a staff profile. The staff member must be deactivated first. Historical data is preserved.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // Service assignment endpoints
        group.MapPost("/{id:guid}/services", async (Guid id, AssignServiceRequest request, ISender sender) =>
        {
            var command = new AssignServiceToStaffCommand(id, request.ServiceId);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("AssignServiceToStaff")
        .WithSummary("Assign service to staff")
        .WithDescription("Assigns a service to a staff member, allowing them to perform that service. The service must belong to the same business as the staff member.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}/services/{serviceId:guid}", async (Guid id, Guid serviceId, ISender sender) =>
        {
            var command = new UnassignServiceFromStaffCommand(id, serviceId);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UnassignServiceFromStaff")
        .WithSummary("Unassign service from staff")
        .WithDescription("Removes a service assignment from a staff member. The staff member will no longer be able to perform this service.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/by-service/{serviceId:guid}", async (Guid serviceId, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffByServiceQuery(serviceId));
            return result.ToApiResponse();
        })
        .WithName("GetStaffByService")
        .WithSummary("Get staff by service")
        .WithDescription("Retrieves all active staff members who can perform a specific service. Useful for showing available staff during booking.")
        .Produces<ApiResponse<IReadOnlyList<StaffProfileResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        // Time off endpoints
        group.MapGet("/{id:guid}/time-off", async (Guid id, DateOnly? startDate, DateOnly? endDate, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffTimeOffQuery(id, startDate, endDate));
            return result.ToApiResponse();
        })
        .WithName("GetStaffTimeOff")
        .WithSummary("Get staff time off")
        .WithDescription("Retrieves all time off requests for a staff member. Optionally filter by date range.")
        .Produces<ApiResponse<IReadOnlyList<TimeOffResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/time-off", async (Guid id, RequestTimeOffRequest request, ISender sender) =>
        {
            var command = new RequestTimeOffCommand(
                id,
                request.StartDate,
                request.EndDate,
                request.Type,
                request.Notes);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("RequestTimeOff")
        .WithSummary("Request time off")
        .WithDescription("Creates a new time off request for a staff member. The request starts in Pending status and must be approved by a manager.")
        .Produces<ApiResponse<RequestTimeOffResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/time-off/{timeOffId:guid}/approve", async (Guid id, Guid timeOffId, ApproveTimeOffRequest request, ISender sender) =>
        {
            var command = new ApproveTimeOffCommand(timeOffId, request.ApprovedByUserId);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("ApproveTimeOff")
        .WithSummary("Approve time off")
        .WithDescription("Approves a pending time off request. Only managers can approve time off requests.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id:guid}/time-off/{timeOffId:guid}/reject", async (Guid id, Guid timeOffId, RejectTimeOffRequest request, ISender sender) =>
        {
            var command = new RejectTimeOffCommand(timeOffId, request.Reason);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("RejectTimeOff")
        .WithSummary("Reject time off")
        .WithDescription("Rejects a pending time off request with an optional reason.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}/time-off/{timeOffId:guid}", async (Guid id, Guid timeOffId, ISender sender) =>
        {
            var result = await sender.Send(new CancelTimeOffCommand(timeOffId));
            return result.ToApiResponse();
        })
        .WithName("CancelTimeOff")
        .WithSummary("Cancel time off")
        .WithDescription("Cancels a time off request. Can only be cancelled if it hasn't started yet.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // Blocked time endpoints
        group.MapGet("/{id:guid}/blocked-times", async (Guid id, DateOnly? startDate, DateOnly? endDate, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffBlockedTimesQuery(id, startDate, endDate));
            return result.ToApiResponse();
        })
        .WithName("GetStaffBlockedTimes")
        .WithSummary("Get staff blocked times")
        .WithDescription("Retrieves all blocked times for a staff member. Includes both recurring and one-time blocks. Optionally filter by date range.")
        .Produces<ApiResponse<IReadOnlyList<BlockedTimeResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/blocked-times", async (Guid id, CreateBlockedTimeRequest request, ISender sender) =>
        {
            var command = new CreateBlockedTimeCommand(
                id,
                request.Title,
                request.StartTime,
                request.EndTime,
                request.IsRecurring,
                request.DayOfWeek,
                request.SpecificDate);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("CreateBlockedTime")
        .WithSummary("Create blocked time")
        .WithDescription("Creates a new blocked time for a staff member. Can be recurring (repeats weekly) or one-time (specific date).")
        .Produces<ApiResponse<CreateBlockedTimeResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}/blocked-times/{blockedTimeId:guid}", async (Guid id, Guid blockedTimeId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteBlockedTimeCommand(blockedTimeId));
            return result.ToApiResponse();
        })
        .WithName("DeleteBlockedTime")
        .WithSummary("Delete blocked time")
        .WithDescription("Deletes a blocked time from a staff member's schedule.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // Availability endpoints
        group.MapGet("/{id:guid}/availability", async (Guid id, DateOnly startDate, DateOnly endDate, ISender sender) =>
        {
            var result = await sender.Send(new GetStaffAvailabilityQuery(id, startDate, endDate));
            return result.ToApiResponse();
        })
        .WithName("GetStaffAvailability")
        .WithSummary("Get staff availability")
        .WithDescription("Calculates the available time slots for a staff member within a date range. Takes into account the default schedule, approved time off, and blocked times.")
        .Produces<ApiResponse<StaffAvailabilityResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/my-schedule", async (Guid userId, Guid businessId, ISender sender) =>
        {
            var result = await sender.Send(new GetMyScheduleQuery(userId, businessId));
            return result.ToApiResponse();
        })
        .WithName("GetMySchedule")
        .WithSummary("Get my schedule")
        .WithDescription("Retrieves the current user's weekly schedule. The user must have a staff profile in the specified business.")
        .Produces<ApiResponse<StaffScheduleResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request DTO for creating a staff profile.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
/// <param name="UserId">The user ID to create a staff profile for.</param>
/// <param name="Title">The staff member's title/role (required, max 100 characters).</param>
/// <param name="Bio">The optional bio/description (max 1000 characters).</param>
/// <param name="ProfileImageUrl">The optional profile image URL (max 500 characters).</param>
/// <param name="DisplayOrder">The display order for sorting (0 or higher).</param>
/// <param name="AcceptsOnlineBookings">Whether the staff accepts online bookings.</param>
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
/// <param name="Title">The staff member's title/role (required, max 100 characters).</param>
/// <param name="Bio">The optional bio/description (max 1000 characters).</param>
/// <param name="ProfileImageUrl">The optional profile image URL (max 500 characters).</param>
/// <param name="DisplayOrder">The display order for sorting (0 or higher).</param>
/// <param name="AcceptsOnlineBookings">Whether the staff accepts online bookings.</param>
public sealed record UpdateStaffRequest(
    string Title,
    string? Bio,
    string? ProfileImageUrl,
    int DisplayOrder,
    bool AcceptsOnlineBookings);

/// <summary>
/// Request DTO for updating a staff schedule.
/// </summary>
/// <param name="Schedule">The schedule for each day of the week (use day names as keys, e.g., "Monday").</param>
public sealed record UpdateStaffScheduleRequest(
    Dictionary<string, WorkDayInputDto?> Schedule);

/// <summary>
/// Input DTO for a single work day in the schedule.
/// </summary>
/// <param name="StartTime">The work start time in HH:mm format.</param>
/// <param name="EndTime">The work end time in HH:mm format.</param>
/// <param name="BreakStart">The optional break start time in HH:mm format.</param>
/// <param name="BreakEnd">The optional break end time in HH:mm format.</param>
public sealed record WorkDayInputDto(
    string StartTime,
    string EndTime,
    string? BreakStart,
    string? BreakEnd);

/// <summary>
/// Request DTO for assigning a service to a staff member.
/// </summary>
/// <param name="ServiceId">The ID of the service to assign.</param>
public sealed record AssignServiceRequest(Guid ServiceId);

/// <summary>
/// Request DTO for requesting time off.
/// </summary>
/// <param name="StartDate">The start date of the time off.</param>
/// <param name="EndDate">The end date of the time off (inclusive).</param>
/// <param name="Type">The type of time off.</param>
/// <param name="Notes">Optional notes for the request.</param>
public sealed record RequestTimeOffRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOffType Type,
    string? Notes);

/// <summary>
/// Request DTO for approving time off.
/// </summary>
/// <param name="ApprovedByUserId">The ID of the user approving the request.</param>
public sealed record ApproveTimeOffRequest(Guid ApprovedByUserId);

/// <summary>
/// Request DTO for rejecting time off.
/// </summary>
/// <param name="Reason">The optional reason for rejection.</param>
public sealed record RejectTimeOffRequest(string? Reason);

/// <summary>
/// Request DTO for creating a blocked time.
/// </summary>
/// <param name="Title">Optional title/reason for the blocked time.</param>
/// <param name="StartTime">The start time.</param>
/// <param name="EndTime">The end time.</param>
/// <param name="IsRecurring">Whether this is a recurring blocked time.</param>
/// <param name="DayOfWeek">The day of week for recurring blocked time.</param>
/// <param name="SpecificDate">The specific date for one-time blocked time.</param>
public sealed record CreateBlockedTimeRequest(
    string? Title,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsRecurring,
    DayOfWeek? DayOfWeek,
    DateOnly? SpecificDate);
