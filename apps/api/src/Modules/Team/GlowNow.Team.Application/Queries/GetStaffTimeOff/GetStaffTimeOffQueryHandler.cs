using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Queries.GetStaffTimeOff;

/// <summary>
/// Handler for the GetStaffTimeOffQuery.
/// </summary>
internal sealed class GetStaffTimeOffQueryHandler
    : IQueryHandler<GetStaffTimeOffQuery, IReadOnlyList<TimeOffResponse>>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITimeOffRepository _timeOffRepository;

    public GetStaffTimeOffQueryHandler(
        IStaffProfileRepository staffProfileRepository,
        ITimeOffRepository timeOffRepository)
    {
        _staffProfileRepository = staffProfileRepository;
        _timeOffRepository = timeOffRepository;
    }

    public async Task<Result<IReadOnlyList<TimeOffResponse>>> Handle(
        GetStaffTimeOffQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            query.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<IReadOnlyList<TimeOffResponse>>(TeamErrors.StaffProfileNotFound);
        }

        IReadOnlyList<TimeOff> timeOffs;

        if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            timeOffs = await _timeOffRepository.GetByStaffProfileIdAndDateRangeAsync(
                query.StaffProfileId,
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else
        {
            timeOffs = await _timeOffRepository.GetByStaffProfileIdAsync(
                query.StaffProfileId, cancellationToken);
        }

        var response = timeOffs
            .Select(t => new TimeOffResponse(
                t.Id,
                t.StaffProfileId,
                t.StartDate,
                t.EndDate,
                t.Type.ToString(),
                t.Notes,
                t.Status.ToString(),
                t.RequestedAtUtc,
                t.ApprovedAtUtc,
                t.ApprovedByUserId,
                t.RejectionReason,
                t.DaysCount))
            .ToList();

        return response;
    }
}
