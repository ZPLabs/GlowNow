using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Application.Queries.GetBusinessDetails;
using GlowNow.Business.Domain.Errors;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Shared.Domain.Errors;

namespace GlowNow.Business.Application.Queries.GetOperatingHours;

/// <summary>
/// Handler for the GetOperatingHoursQuery.
/// </summary>
internal sealed class GetOperatingHoursQueryHandler : IQueryHandler<GetOperatingHoursQuery, OperatingHoursResponse>
{
    private readonly IBusinessRepository _businessRepository;

    public GetOperatingHoursQueryHandler(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<Result<OperatingHoursResponse>> Handle(
        GetOperatingHoursQuery query,
        CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(query.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Failure<OperatingHoursResponse>(BusinessErrors.BusinessNotFound);
        }

        var schedule = new Dictionary<DayOfWeek, TimeRangeResponse?>();

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            var timeRange = business.OperatingHours.GetHoursForDay(day);
            schedule[day] = timeRange is null
                ? null
                : new TimeRangeResponse(
                    timeRange.OpenTime.ToString("HH:mm"),
                    timeRange.CloseTime.ToString("HH:mm"));
        }

        return new OperatingHoursResponse(schedule);
    }
}
