# GlowNow API

.NET 10 REST API following clean architecture, serving as the backend for all GlowNow clients (web and mobile).

## Architecture

The solution follows a four-layer clean architecture:

```
GlowNow.Api              Web host, endpoints, middleware
  -> GlowNow.Application   Use cases, service interfaces, DTOs
  -> GlowNow.Infrastructure External concerns (database, email, SMS)
  -> GlowNow.Domain         Entities, value objects, domain logic
```

**Dependency rule:** Dependencies point inward. Domain has no external references. Application depends only on Domain. Infrastructure implements Application interfaces.

## Tech Stack

- **.NET 10** (SDK 10.0.100)
- **Minimal API** style endpoints
- **Warnings as errors** enforced via `Directory.Build.props`

## Development

```bash
# From the repo root
npx turbo dev --filter=api

# Or from this directory
dotnet run --project src/GlowNow.Api
```

The API runs on [http://localhost:5249](http://localhost:5249).

## Scripts

```bash
dotnet build GlowNow.Api.sln                    # Build the solution
dotnet build GlowNow.Api.sln --configuration Release  # Release build
dotnet run --project src/GlowNow.Api             # Run the API
dotnet format GlowNow.Api.sln --verify-no-changes     # Check formatting
```

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/health` | Health check — returns `{"status":"healthy"}` |

## Planned Responsibilities

This API will handle:

- **Authentication** — Email/password auth, JWT tokens, role-based access
- **Multi-tenancy** — Strict data isolation between businesses
- **Business onboarding** — Registration with RUC validation (Ecuador tax ID)
- **Service catalog** — CRUD for services, categories, pricing, duration
- **Team management** — Staff profiles, permission levels, service assignments
- **Scheduling** — Shift patterns, time-off, blocked time
- **Booking engine** — Real-time availability calculation, appointment lifecycle
- **Notifications** — Email (SendGrid/SES) and SMS (Twilio) dispatch
