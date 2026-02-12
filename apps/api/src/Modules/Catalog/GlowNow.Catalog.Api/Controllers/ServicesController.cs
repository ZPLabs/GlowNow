using GlowNow.Catalog.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using GlowNow.Catalog.Application.Commands.CreateService;
using GlowNow.Catalog.Application.Commands.DeleteService;
using GlowNow.Catalog.Application.Commands.UpdateService;
using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Catalog.Application.Queries.GetService;
using GlowNow.Catalog.Application.Queries.GetServicesByCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlowNow.Catalog.Api.Controllers;

/// <summary>
/// API controller for service management.
/// </summary>
[ApiController]
[Route("api/v1/services")]
[Produces("application/json")]
[Authorize]
public class ServicesController : ControllerBase
{
    private readonly ISender _sender;

    public ServicesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create service.
    /// </summary>
    /// <remarks>
    /// Creates a new bookable service for a business. The service name must be unique within the business. Services can optionally be assigned to a category. Duration must be between 5 and 480 minutes. Price is in USD.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateServiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest request)
    {
        var command = new CreateServiceCommand(
            request.BusinessId,
            request.CategoryId,
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.Price,
            request.BufferTimeMinutes,
            request.DisplayOrder);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// List services.
    /// </summary>
    /// <remarks>
    /// Retrieves all services for a business, ordered by display order and then by name. Only active (non-deleted) services are returned. Includes all service details like duration, price, and category.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ServiceResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllServices([FromQuery] Guid businessId)
    {
        var result = await _sender.Send(new GetAllServicesQuery(businessId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get service.
    /// </summary>
    /// <remarks>
    /// Retrieves the full details of a specific service including name, description, duration, price, buffer time, category assignment, and active status.
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ServiceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetService(Guid id)
    {
        var result = await _sender.Send(new GetServiceQuery(id));
        return result.ToActionResult();
    }

    /// <summary>
    /// List services by category.
    /// </summary>
    /// <remarks>
    /// Retrieves all services belonging to a specific category, ordered by display order and then by name. Only active (non-deleted) services are returned.
    /// </remarks>
    [HttpGet("by-category/{categoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ServiceResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetServicesByCategory(Guid categoryId)
    {
        var result = await _sender.Send(new GetServicesByCategoryQuery(categoryId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Update service.
    /// </summary>
    /// <remarks>
    /// Updates an existing service. The service name must remain unique within the business. Category can be changed or removed (set to null). Duration must be between 5 and 480 minutes. Price is in USD.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateService(Guid id, [FromBody] UpdateServiceRequest request)
    {
        var command = new UpdateServiceCommand(
            id,
            request.CategoryId,
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.Price,
            request.BufferTimeMinutes,
            request.DisplayOrder);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete service.
    /// </summary>
    /// <remarks>
    /// Soft-deletes a service. The service will be deactivated and marked as deleted. It will no longer appear in listings but historical booking data will be preserved.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteService(Guid id)
    {
        var result = await _sender.Send(new DeleteServiceCommand(id));
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO for creating a service.
/// </summary>
public sealed record CreateServiceRequest(
    Guid BusinessId,
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder);

/// <summary>
/// Request DTO for updating a service.
/// </summary>
public sealed record UpdateServiceRequest(
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder);
