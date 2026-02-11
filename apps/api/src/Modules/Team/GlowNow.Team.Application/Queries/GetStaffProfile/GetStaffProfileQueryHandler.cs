using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Application.Queries.GetAllStaff;

namespace GlowNow.Team.Application.Queries.GetStaffProfile;

/// <summary>
/// Handler for the GetStaffProfileQuery.
/// </summary>
internal sealed class GetStaffProfileQueryHandler : IQueryHandler<GetStaffProfileQuery, StaffProfileResponse>
{
    private readonly IStaffProfileRepository _staffProfileRepository;

    public GetStaffProfileQueryHandler(IStaffProfileRepository staffProfileRepository)
    {
        _staffProfileRepository = staffProfileRepository;
    }

    public async Task<Result<StaffProfileResponse>> Handle(
        GetStaffProfileQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfile = await _staffProfileRepository.GetByIdWithServicesAsync(
            query.Id, cancellationToken);

        if (staffProfile is null)
        {
            return Result.Failure<StaffProfileResponse>(TeamErrors.StaffProfileNotFound);
        }

        return StaffProfileResponse.FromEntity(staffProfile);
    }
}
