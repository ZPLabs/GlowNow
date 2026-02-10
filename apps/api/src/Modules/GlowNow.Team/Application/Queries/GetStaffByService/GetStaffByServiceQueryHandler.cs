using GlowNow.Shared.Application.Messaging;
using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Application.Queries.GetAllStaff;

namespace GlowNow.Team.Application.Queries.GetStaffByService;

/// <summary>
/// Handler for the GetStaffByServiceQuery.
/// </summary>
internal sealed class GetStaffByServiceQueryHandler
    : IQueryHandler<GetStaffByServiceQuery, IReadOnlyList<StaffProfileResponse>>
{
    private readonly IStaffProfileRepository _staffProfileRepository;

    public GetStaffByServiceQueryHandler(IStaffProfileRepository staffProfileRepository)
    {
        _staffProfileRepository = staffProfileRepository;
    }

    public async Task<Result<IReadOnlyList<StaffProfileResponse>>> Handle(
        GetStaffByServiceQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfiles = await _staffProfileRepository.GetByServiceIdAsync(
            query.ServiceId, cancellationToken);

        var response = staffProfiles
            .Select(StaffProfileResponse.FromEntity)
            .ToList();

        return response;
    }
}
