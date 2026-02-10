using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Domain.Errors;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Shared.Domain.Errors;

namespace GlowNow.Business.Application.Queries.GetBusinessDetails;

/// <summary>
/// Handler for the GetBusinessDetailsQuery.
/// </summary>
internal sealed class GetBusinessDetailsQueryHandler : IQueryHandler<GetBusinessDetailsQuery, BusinessDetailsResponse>
{
    private readonly IBusinessRepository _businessRepository;

    public GetBusinessDetailsQueryHandler(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<Result<BusinessDetailsResponse>> Handle(
        GetBusinessDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var business = await _businessRepository.GetByIdAsync(query.BusinessId, cancellationToken);

        if (business is null)
        {
            return Result.Failure<BusinessDetailsResponse>(BusinessErrors.BusinessNotFound);
        }

        var operatingHoursResponse = MapOperatingHours(business.OperatingHours);

        return new BusinessDetailsResponse(
            business.Id,
            business.Name,
            business.Ruc.Value,
            business.Address,
            business.PhoneNumber?.Value,
            business.Email.Value,
            business.Description,
            business.LogoUrl,
            operatingHoursResponse,
            business.CreatedAtUtc);
    }

    private static OperatingHoursResponse MapOperatingHours(Domain.ValueObjects.OperatingHours operatingHours)
    {
        var schedule = new Dictionary<DayOfWeek, TimeRangeResponse?>();

        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            var timeRange = operatingHours.GetHoursForDay(day);
            schedule[day] = timeRange is null
                ? null
                : new TimeRangeResponse(
                    timeRange.OpenTime.ToString("HH:mm"),
                    timeRange.CloseTime.ToString("HH:mm"));
        }

        return new OperatingHoursResponse(schedule);
    }
}
