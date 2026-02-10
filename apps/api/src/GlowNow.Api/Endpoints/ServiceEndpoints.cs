using GlowNow.Api.Infrastructure;
using GlowNow.Catalog.Application.Commands.CreateService;
using GlowNow.Catalog.Application.Commands.DeleteService;
using GlowNow.Catalog.Application.Commands.UpdateService;
using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Catalog.Application.Queries.GetService;
using GlowNow.Catalog.Application.Queries.GetServicesByCategory;
using MediatR;

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for service management.
/// </summary>
public static class ServiceEndpoints
{
    public static void MapServiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/services")
            .WithTags("Services")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/", async (CreateServiceRequest request, ISender sender) =>
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
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("CreateService")
        .WithSummary("Create service")
        .WithDescription("Creates a new bookable service for a business. The service name must be unique within the business. Services can optionally be assigned to a category. Duration must be between 5 and 480 minutes. Price is in USD.")
        .Produces<ApiResponse<CreateServiceResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", async (Guid businessId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllServicesQuery(businessId));
            return result.ToApiResponse();
        })
        .WithName("GetAllServices")
        .WithSummary("List services")
        .WithDescription("Retrieves all services for a business, ordered by display order and then by name. Only active (non-deleted) services are returned. Includes all service details like duration, price, and category.")
        .Produces<ApiResponse<IReadOnlyList<ServiceResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetServiceQuery(id));
            return result.ToApiResponse();
        })
        .WithName("GetService")
        .WithSummary("Get service")
        .WithDescription("Retrieves the full details of a specific service including name, description, duration, price, buffer time, category assignment, and active status.")
        .Produces<ApiResponse<ServiceResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/by-category/{categoryId:guid}", async (Guid categoryId, ISender sender) =>
        {
            var result = await sender.Send(new GetServicesByCategoryQuery(categoryId));
            return result.ToApiResponse();
        })
        .WithName("GetServicesByCategory")
        .WithSummary("List services by category")
        .WithDescription("Retrieves all services belonging to a specific category, ordered by display order and then by name. Only active (non-deleted) services are returned.")
        .Produces<ApiResponse<IReadOnlyList<ServiceResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", async (Guid id, UpdateServiceRequest request, ISender sender) =>
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
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateService")
        .WithSummary("Update service")
        .WithDescription("Updates an existing service. The service name must remain unique within the business. Category can be changed or removed (set to null). Duration must be between 5 and 480 minutes. Price is in USD.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteServiceCommand(id));
            return result.ToApiResponse();
        })
        .WithName("DeleteService")
        .WithSummary("Delete service")
        .WithDescription("Soft-deletes a service. The service will be deactivated and marked as deleted. It will no longer appear in listings but historical booking data will be preserved.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

/// <summary>
/// Request DTO for creating a service.
/// </summary>
/// <param name="BusinessId">The ID of the business to create the service for.</param>
/// <param name="CategoryId">The optional category ID to assign the service to.</param>
/// <param name="Name">The service name (required, max 200 characters, must be unique within business).</param>
/// <param name="Description">The service description (optional, max 1000 characters).</param>
/// <param name="DurationMinutes">The service duration in minutes (5-480).</param>
/// <param name="Price">The service price in USD (0-999999.99).</param>
/// <param name="BufferTimeMinutes">Buffer time in minutes after the service (0 or higher).</param>
/// <param name="DisplayOrder">The display order for sorting services in the UI (0 or higher).</param>
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
/// <param name="CategoryId">The optional category ID to assign the service to (null to remove from category).</param>
/// <param name="Name">The service name (required, max 200 characters, must be unique within business).</param>
/// <param name="Description">The service description (optional, max 1000 characters).</param>
/// <param name="DurationMinutes">The service duration in minutes (5-480).</param>
/// <param name="Price">The service price in USD (0-999999.99).</param>
/// <param name="BufferTimeMinutes">Buffer time in minutes after the service (0 or higher).</param>
/// <param name="DisplayOrder">The display order for sorting services in the UI (0 or higher).</param>
public sealed record UpdateServiceRequest(
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    int DisplayOrder);
