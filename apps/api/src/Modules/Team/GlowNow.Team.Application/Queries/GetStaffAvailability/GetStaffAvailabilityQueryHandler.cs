using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Domain.Services;

namespace GlowNow.Team.Application.Queries.GetStaffAvailability;

/// <summary>
/// Handler for the GetStaffAvailabilityQuery.
/// </summary>
internal sealed class GetStaffAvailabilityQueryHandler
    : IQueryHandler<GetStaffAvailabilityQuery, StaffAvailabilityResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IBlockedTimeRepository _blockedTimeRepository;

    public GetStaffAvailabilityQueryHandler(
        IStaffProfileRepository staffProfileRepository,
        ITimeOffRepository timeOffRepository,
        IBlockedTimeRepository blockedTimeRepository)
    {
        _staffProfileRepository = staffProfileRepository;
        _timeOffRepository = timeOffRepository;
        _blockedTimeRepository = blockedTimeRepository;
    }

    public async Task<Result<StaffAvailabilityResponse>> Handle(
        GetStaffAvailabilityQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdAsync(
            query.StaffProfileId, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<StaffAvailabilityResponse>(TeamErrors.StaffProfileNotFound);
        }

        // Get approved time off for the date range
        var timeOffs = await _timeOffRepository.GetApprovedByStaffProfileIdAndDateRangeAsync(
            query.StaffProfileId,
            query.StartDate,
            query.EndDate,
            cancellationToken);

        // Get blocked times for the date range
        var blockedTimes = await _blockedTimeRepository.GetByStaffProfileIdAndDateRangeAsync(
            query.StaffProfileId,
            query.StartDate,
            query.EndDate,
            cancellationToken);

        // Calculate availability
        var calculator = new AvailabilityCalculator();
        var availability = calculator.CalculateAvailabilityRange(
            query.StartDate,
            query.EndDate,
            staffProfile.DefaultSchedule,
            timeOffs,
            blockedTimes);

        // Convert to response format
        var responseAvailability = availability.ToDictionary(
            kvp => kvp.Key.ToString("yyyy-MM-dd"),
            kvp => (IReadOnlyList<TimeSlotResponse>)kvp.Value
                .Select(slot => new TimeSlotResponse(
                    slot.Start.ToString("HH:mm"),
                    slot.End.ToString("HH:mm")))
                .ToList());

        return new StaffAvailabilityResponse(
            staffProfile.Id,
            query.StartDate,
            query.EndDate,
            responseAvailability);
    }
}
