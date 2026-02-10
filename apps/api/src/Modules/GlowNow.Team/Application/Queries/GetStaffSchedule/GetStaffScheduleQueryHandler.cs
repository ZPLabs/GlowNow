using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Queries.GetStaffSchedule;

/// <summary>
/// Handler for the GetStaffScheduleQuery.
/// </summary>
internal sealed class GetStaffScheduleQueryHandler
    : IQueryHandler<GetStaffScheduleQuery, StaffScheduleResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;

    public GetStaffScheduleQueryHandler(IStaffProfileRepository staffProfileRepository)
    {
        _staffProfileRepository = staffProfileRepository;
    }

    public async Task<Result<StaffScheduleResponse>> Handle(
        GetStaffScheduleQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            query.StaffProfileId, cancellationToken);

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
