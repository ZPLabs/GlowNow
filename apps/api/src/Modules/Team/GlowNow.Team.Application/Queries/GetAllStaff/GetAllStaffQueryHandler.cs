using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Application.Queries.GetAllStaff;

/// <summary>
/// Handler for the GetAllStaffQuery.
/// </summary>
internal sealed class GetAllStaffQueryHandler
    : IQueryHandler<GetAllStaffQuery, IReadOnlyList<StaffProfileResponse>>
{
    private readonly IStaffProfileRepository _staffProfileRepository;

    public GetAllStaffQueryHandler(IStaffProfileRepository staffProfileRepository)
    {
        _staffProfileRepository = staffProfileRepository;
    }

    public async Task<Result<IReadOnlyList<StaffProfileResponse>>> Handle(
        GetAllStaffQuery query,
        CancellationToken cancellationToken)
    {
        var staffProfiles = query.ActiveOnly
            ? await _staffProfileRepository.GetActiveByBusinessIdAsync(
                query.BusinessId, cancellationToken)
            : await _staffProfileRepository.GetAllByBusinessIdAsync(
                query.BusinessId, cancellationToken);

        var response = staffProfiles
            .Select(StaffProfileResponse.FromEntity)
            .ToList();

        return response;
    }
}
