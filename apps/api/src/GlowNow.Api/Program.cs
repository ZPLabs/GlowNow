using GlowNow.Shared;
using GlowNow.Shared.Infrastructure;
using GlowNow.Identity;
using GlowNow.Business;
using GlowNow.Catalog;
using GlowNow.Team;
using GlowNow.Clients;
using GlowNow.Booking;
using GlowNow.Notifications;
using GlowNow.Api.Endpoints;
using GlowNow.Api.Middleware;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var moduleAssemblies = new[]
{
    typeof(IdentityModule).Assembly,
    typeof(BusinessModule).Assembly,
    typeof(CatalogModule).Assembly,
    typeof(TeamModule).Assembly,
    typeof(ClientsModule).Assembly,
    typeof(BookingModule).Assembly,
    typeof(NotificationsModule).Assembly
};

builder.Services.AddSharedServices(moduleAssemblies);
builder.Services.AddSharedInfrastructure(builder.Configuration, moduleAssemblies);

builder.Services
    .AddIdentityModule(builder.Configuration)
    .AddBusinessModule()
    .AddCatalogModule()
    .AddTeamModule()
    .AddClientsModule()
    .AddBookingModule()
    .AddNotificationsModule();

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
app.MapAuthEndpoints();
app.MapBusinessEndpoints();
app.MapServiceCategoryEndpoints();
app.MapServiceEndpoints();
app.MapStaffEndpoints();

app.Run();
