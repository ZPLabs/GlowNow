using GlowNow.Business.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using GlowNow.Business.Application.Commands.SetOperatingHours;
using GlowNow.Business.Application.Commands.UpdateBusinessSettings;
using GlowNow.Business.Application.Queries.GetBusinessDetails;
using GlowNow.Business.Application.Queries.GetOperatingHours;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlowNow.Business.Api.Controllers;

/// <summary>
/// API controller for business management.
/// </summary>
[ApiController]
[Route("api/v1/businesses")]
[Produces("application/json")]
[Authorize]
public class BusinessesController : ControllerBase
{
    private readonly ISender _sender;

    public BusinessesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get business details.
    /// </summary>
    /// <remarks>
    /// Retrieves the full details of a business including name, RUC, address, contact information, description, logo URL, and weekly operating hours schedule.
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BusinessDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBusinessDetails(Guid id)
    {
        var result = await _sender.Send(new GetBusinessDetailsQuery(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get operating hours.
    /// </summary>
    /// <remarks>
    /// Retrieves the weekly operating hours schedule for a business. Each day of the week can have an opening and closing time, or be marked as closed (null).
    /// </remarks>
    [HttpGet("{id:guid}/operating-hours")]
    [ProducesResponseType(typeof(ApiResponse<OperatingHoursResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOperatingHours(Guid id)
    {
        var result = await _sender.Send(new GetOperatingHoursQuery(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// Set operating hours.
    /// </summary>
    /// <remarks>
    /// Sets the weekly operating hours schedule for a business. Provide a schedule mapping each day of the week to opening/closing times. Set a day to null to mark it as closed.
    /// </remarks>
    [HttpPut("{id:guid}/operating-hours")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetOperatingHours(Guid id, [FromBody] SetOperatingHoursRequest request)
    {
        var command = new SetOperatingHoursCommand(id, request.Schedule);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Update business settings.
    /// </summary>
    /// <remarks>
    /// Updates the business profile settings including name (required), description (optional), and logo URL (optional, must be a valid absolute URL).
    /// </remarks>
    [HttpPut("{id:guid}/settings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBusinessSettings(Guid id, [FromBody] UpdateBusinessSettingsRequest request)
    {
        var command = new UpdateBusinessSettingsCommand(id, request.Name, request.Description, request.LogoUrl);
        var result = await _sender.Send(command);
        return result.ToActionResult();
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
