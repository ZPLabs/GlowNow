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
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetBusinessDetailsQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetBusinessDetails")
        .WithSummary("Get business details")
        .WithDescription("Retrieves the full details of a business including name, RUC, address, contact information, description, logo URL, and weekly operating hours schedule.")
        .Produces<ApiResponse<BusinessDetailsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/operating-hours", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetOperatingHoursQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetOperatingHours")
        .WithSummary("Get operating hours")
        .WithDescription("Retrieves the weekly operating hours schedule for a business. Each day of the week can have an opening and closing time, or be marked as closed (null).")
        .Produces<ApiResponse<OperatingHoursResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/operating-hours", async (Guid id, SetOperatingHoursRequest request, ISender sender) =>
        {
            var command = new SetOperatingHoursCommand(id, request.Schedule);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("SetOperatingHours")
        .WithSummary("Set operating hours")
        .WithDescription("Sets the weekly operating hours schedule for a business. Provide a schedule mapping each day of the week to opening/closing times. Set a day to null to mark it as closed.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}/settings", async (Guid id, UpdateBusinessSettingsRequest request, ISender sender) =>
        {
            var command = new UpdateBusinessSettingsCommand(id, request.Name, request.Description, request.LogoUrl);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateBusinessSettings")
        .WithSummary("Update business settings")
        .WithDescription("Updates the business profile settings including name (required), description (optional), and logo URL (optional, must be a valid absolute URL).")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request DTO for setting operating hours.
/// </summary>
/// <param name="Schedule">The weekly schedule mapping day of week (0=Sunday to 6=Saturday) to operating hours. Set a day to null to mark it as closed.</param>
public sealed record SetOperatingHoursRequest(Dictionary<DayOfWeek, DayScheduleDto?> Schedule);

/// <summary>
/// Request DTO for updating business settings.
/// </summary>
/// <param name="Name">The business name (required, max 200 characters).</param>
/// <param name="Description">The business description (optional, max 1000 characters).</param>
/// <param name="LogoUrl">The business logo URL (optional, must be a valid absolute URL).</param>
public sealed record UpdateBusinessSettingsRequest(string Name, string? Description, string? LogoUrl);
