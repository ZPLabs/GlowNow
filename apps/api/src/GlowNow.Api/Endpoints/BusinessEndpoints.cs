using GlowNow.Api.Infrastructure;
using GlowNow.Business.Application.Commands.SetOperatingHours;
using GlowNow.Business.Application.Commands.UpdateBusinessSettings;
using GlowNow.Business.Application.Queries.GetBusinessDetails;
using GlowNow.Business.Application.Queries.GetOperatingHours;
using MediatR;

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for business management.
/// </summary>
public static class BusinessEndpoints
{
    public static void MapBusinessEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/businesses")
            .WithTags("Business")
            .RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetBusinessDetailsQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetBusinessDetails")
        .WithDescription("Get full details of a business including operating hours")
        .Produces<ApiResponse<BusinessDetailsResponse>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/operating-hours", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetOperatingHoursQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetOperatingHours")
        .WithDescription("Get the weekly operating hours for a business")
        .Produces<ApiResponse<OperatingHoursResponse>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/operating-hours", async (Guid id, SetOperatingHoursRequest request, ISender sender) =>
        {
            var command = new SetOperatingHoursCommand(id, request.Schedule);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("SetOperatingHours")
        .WithDescription("Set the weekly operating hours for a business")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/settings", async (Guid id, UpdateBusinessSettingsRequest request, ISender sender) =>
        {
            var command = new UpdateBusinessSettingsCommand(id, request.Name, request.Description, request.LogoUrl);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateBusinessSettings")
        .WithDescription("Update business settings like name, description, and logo")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request DTO for setting operating hours.
/// </summary>
/// <param name="Schedule">The weekly schedule mapping day of week to operating hours.</param>
public sealed record SetOperatingHoursRequest(Dictionary<DayOfWeek, DayScheduleDto?> Schedule);

/// <summary>
/// Request DTO for updating business settings.
/// </summary>
/// <param name="Name">The new business name.</param>
/// <param name="Description">The new business description.</param>
/// <param name="LogoUrl">The new logo URL.</param>
public sealed record UpdateBusinessSettingsRequest(string Name, string? Description, string? LogoUrl);
