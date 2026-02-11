using GlowNow.Infrastructure.Core;
using GlowNow.Identity.Api;
using GlowNow.Business.Api;
using GlowNow.Catalog.Api;
using GlowNow.Team.Api;
using GlowNow.Clients.Api;
using GlowNow.Booking.Api;
using GlowNow.Notifications.Api;
using GlowNow.Api.Middleware;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var moduleAssemblies = new[]
{
    typeof(GlowNow.Identity.Api.IdentityModule).Assembly,
    typeof(GlowNow.Business.Api.BusinessModule).Assembly,
    typeof(GlowNow.Catalog.Api.CatalogModule).Assembly,
    typeof(GlowNow.Team.Api.TeamModule).Assembly,
    typeof(GlowNow.Clients.Api.ClientsModule).Assembly,
    typeof(GlowNow.Booking.Api.BookingModule).Assembly,
    typeof(GlowNow.Notifications.Api.NotificationsModule).Assembly
};

// Register Infrastructure.Core services (MediatR, validation behaviors, etc.)
builder.Services.AddInfrastructureCore(moduleAssemblies);

// Register module-specific services
builder.Services
    .AddIdentityModule(builder.Configuration)
    .AddBusinessModule(builder.Configuration)
    .AddCatalogModule(builder.Configuration)
    .AddTeamModule(builder.Configuration)
    .AddClientsModule(builder.Configuration)
    .AddBookingModule(builder.Configuration)
    .AddNotificationsModule();

// Add MVC Controllers
builder.Services.AddControllers()
    .AddApplicationPart(typeof(GlowNow.Identity.Api.IdentityModule).Assembly)
    .AddApplicationPart(typeof(GlowNow.Business.Api.BusinessModule).Assembly)
    .AddApplicationPart(typeof(GlowNow.Catalog.Api.CatalogModule).Assembly)
    .AddApplicationPart(typeof(GlowNow.Team.Api.TeamModule).Assembly)
    .AddApplicationPart(typeof(GlowNow.Clients.Api.ClientsModule).Assembly)
    .AddApplicationPart(typeof(GlowNow.Booking.Api.BookingModule).Assembly)
    .AddApplicationPart(typeof(GlowNow.Notifications.Api.NotificationsModule).Assembly);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var cognitoSection = builder.Configuration.GetSection("Cognito");
        string region = cognitoSection["Region"] ?? "us-east-1";
        string userPoolId = cognitoSection["UserPoolId"] ?? "";

        options.Authority = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://cognito-idp.{region}.amazonaws.com/{userPoolId}",
            ValidateAudience = false, // Cognito access tokens don't have an aud claim by default
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();
app.UseMiddleware<TenantMiddleware>();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.EndpointPathPrefix = "/scalar/{documentName}";
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// Map controllers from all modules
app.MapControllers();

app.Run();
