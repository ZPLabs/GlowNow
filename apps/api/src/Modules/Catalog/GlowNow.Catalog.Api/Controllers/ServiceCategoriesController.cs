using GlowNow.Catalog.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using GlowNow.Catalog.Application.Commands.CreateServiceCategory;
using GlowNow.Catalog.Application.Commands.DeleteServiceCategory;
using GlowNow.Catalog.Application.Commands.UpdateServiceCategory;
using GlowNow.Catalog.Application.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlowNow.Catalog.Api.Controllers;

/// <summary>
/// API controller for service category management.
/// </summary>
[ApiController]
[Route("api/v1/services/categories")]
[Produces("application/json")]
[Authorize]
public class ServiceCategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public ServiceCategoriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create service category.
    /// </summary>
    /// <remarks>
    /// Creates a new service category for organizing services. Categories help group related services (e.g., 'Haircuts', 'Coloring', 'Treatments'). The name must be unique within the business.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CreateServiceCategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateServiceCategory([FromBody] CreateServiceCategoryRequest request)
    {
        var command = new CreateServiceCategoryCommand(
            request.BusinessId,
            request.Name,
            request.Description,
            request.DisplayOrder);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// List service categories.
    /// </summary>
    /// <remarks>
    /// Retrieves all service categories for a business, ordered by display order and then by name. Only active (non-deleted) categories are returned.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ServiceCategoryResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllCategories([FromQuery] Guid businessId)
    {
        var result = await _sender.Send(new GetAllCategoriesQuery(businessId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Update service category.
    /// </summary>
    /// <remarks>
    /// Updates an existing service category. The category name must remain unique within the business. All services in this category will continue to reference it.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateServiceCategory(Guid id, [FromBody] UpdateServiceCategoryRequest request)
    {
        var command = new UpdateServiceCategoryCommand(
            id,
            request.Name,
            request.Description,
            request.DisplayOrder);
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Delete service category.
    /// </summary>
    /// <remarks>
    /// Soft-deletes a service category. The category cannot be deleted if it still contains services. Move or delete the services first, then delete the category.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteServiceCategory(Guid id)
    {
        var result = await _sender.Send(new DeleteServiceCategoryCommand(id));
        return result.ToActionResult();
    }
}

/// <summary>
/// Request DTO for creating a service category.
/// </summary>
public sealed record CreateServiceCategoryRequest(
    Guid BusinessId,
    string Name,
    string? Description,
    int DisplayOrder);

/// <summary>
/// Request DTO for updating a service category.
/// </summary>
public sealed record UpdateServiceCategoryRequest(
    string Name,
    string? Description,
    int DisplayOrder);
