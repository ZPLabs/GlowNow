using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Application.Queries.GetStaffSchedule;

namespace GlowNow.Team.Application.Queries.GetMySchedule;

/// <summary>
/// Handler for the GetMyScheduleQuery.
/// </summary>
internal sealed class GetMyScheduleQueryHandler
    : IQueryHandler<GetMyScheduleQuery, StaffScheduleResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;

    public GetMyScheduleQueryHandler(IStaffProfileRepository staffProfileRepository)
    {
        _staffProfileRepository = staffProfileRepository;
    }

    public async Task<Result<StaffScheduleResponse>> Handle(
        GetMyScheduleQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByUserIdAsync(
            query.BusinessId, query.UserId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<StaffScheduleResponse>(TeamErrors.StaffProfileNotFound);
        }

        var scheduleDict = new Dictionary<string, WorkDayResponse?>();
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            var workDay = staffProfile.DefaultSchedule.GetWorkDayFor(day);
            scheduleDict[day.ToString()] = workDay is null
                ? null
                : new WorkDayResponse(
                    workDay.StartTime.ToString("HH:mm"),
                    workDay.EndTime.ToString("HH:mm"),
                    workDay.BreakStart?.ToString("HH:mm"),
                    workDay.BreakEnd?.ToString("HH:mm"));
        }

        return new StaffScheduleResponse(staffProfile.Id, scheduleDict);
    }
}
