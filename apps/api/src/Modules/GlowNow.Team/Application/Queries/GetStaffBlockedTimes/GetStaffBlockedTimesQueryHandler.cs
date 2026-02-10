using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Queries.GetStaffBlockedTimes;

/// <summary>
/// Handler for the GetStaffBlockedTimesQuery.
/// </summary>
internal sealed class GetStaffBlockedTimesQueryHandler
    : IQueryHandler<GetStaffBlockedTimesQuery, IReadOnlyList<BlockedTimeResponse>>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly IBlockedTimeRepository _blockedTimeRepository;

    public GetStaffBlockedTimesQueryHandler(
        IStaffProfileRepository staffProfileRepository,
        IBlockedTimeRepository blockedTimeRepository)
    {
        _staffProfileRepository = staffProfileRepository;
        _blockedTimeRepository = blockedTimeRepository;
    }

    public async Task<Result<IReadOnlyList<BlockedTimeResponse>>> Handle(
        GetStaffBlockedTimesQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            query.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<IReadOnlyList<BlockedTimeResponse>>(TeamErrors.StaffProfileNotFound);
        }

        IReadOnlyList<BlockedTime> blockedTimes;

        if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            blockedTimes = await _blockedTimeRepository.GetByStaffProfileIdAndDateRangeAsync(
                query.StaffProfileId,
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else
        {
            blockedTimes = await _blockedTimeRepository.GetByStaffProfileIdAsync(
                query.StaffProfileId, cancellationToken);
        }

        var response = blockedTimes
            .Select(bt => new BlockedTimeResponse(
                bt.Id,
                bt.StaffProfileId,
                bt.Title,
                bt.StartTime.ToString("HH:mm"),
                bt.EndTime.ToString("HH:mm"),
                bt.IsRecurring,
                bt.RecurringDayOfWeek?.ToString(),
                bt.SpecificDate,
                bt.CreatedAtUtc))
            .ToList();

        return response;
    }
}
