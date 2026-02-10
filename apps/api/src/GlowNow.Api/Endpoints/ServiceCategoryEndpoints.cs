using GlowNow.Api.Infrastructure;
using GlowNow.Catalog.Application.Commands.CreateServiceCategory;
using GlowNow.Catalog.Application.Commands.DeleteServiceCategory;
using GlowNow.Catalog.Application.Commands.UpdateServiceCategory;
using GlowNow.Catalog.Application.Queries.GetAllCategories;
using MediatR;

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for service category management.
/// </summary>
public static class ServiceCategoryEndpoints
{
    public static void MapServiceCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/services/categories")
            .WithTags("Service Categories")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/", async (CreateServiceCategoryRequest request, ISender sender) =>
        {
            var command = new CreateServiceCategoryCommand(
                request.BusinessId,
                request.Name,
                request.Description,
                request.DisplayOrder);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("CreateServiceCategory")
        .WithSummary("Create service category")
        .WithDescription("Creates a new service category for organizing services. Categories help group related services (e.g., 'Haircuts', 'Coloring', 'Treatments'). The name must be unique within the business.")
        .Produces<ApiResponse<CreateServiceCategoryResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/", async (Guid businessId, ISender sender) =>
        {
            var result = await sender.Send(new GetAllCategoriesQuery(businessId));
            return result.ToApiResponse();
        })
        .WithName("GetAllCategories")
        .WithSummary("List service categories")
        .WithDescription("Retrieves all service categories for a business, ordered by display order and then by name. Only active (non-deleted) categories are returned.")
        .Produces<ApiResponse<IReadOnlyList<ServiceCategoryResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPut("/{id:guid}", async (Guid id, UpdateServiceCategoryRequest request, ISender sender) =>
        {
            var command = new UpdateServiceCategoryCommand(
                id,
                request.Name,
                request.Description,
                request.DisplayOrder);
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("UpdateServiceCategory")
        .WithSummary("Update service category")
        .WithDescription("Updates an existing service category. The category name must remain unique within the business. All services in this category will continue to reference it.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteServiceCategoryCommand(id));
            return result.ToApiResponse();
        })
        .WithName("DeleteServiceCategory")
        .WithSummary("Delete service category")
        .WithDescription("Soft-deletes a service category. The category cannot be deleted if it still contains services. Move or delete the services first, then delete the category.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

/// <summary>
/// Request DTO for creating a service category.
/// </summary>
/// <param name="BusinessId">The ID of the business to create the category for.</param>
/// <param name="Name">The category name (required, max 100 characters, must be unique within business).</param>
/// <param name="Description">The category description (optional, max 500 characters).</param>
/// <param name="DisplayOrder">The display order for sorting categories in the UI (0 or higher).</param>
public sealed record CreateServiceCategoryRequest(
    Guid BusinessId,
    string Name,
    string? Description,
    int DisplayOrder);

/// <summary>
/// Request DTO for updating a service category.
/// </summary>
/// <param name="Name">The category name (required, max 100 characters, must be unique within business).</param>
/// <param name="Description">The category description (optional, max 500 characters).</param>
/// <param name="DisplayOrder">The display order for sorting categories in the UI (0 or higher).</param>
public sealed record UpdateServiceCategoryRequest(
    string Name,
    string? Description,
    int DisplayOrder);
