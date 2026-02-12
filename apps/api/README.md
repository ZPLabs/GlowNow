# GlowNow API

.NET 10 REST API following modular monolith architecture with Clean Architecture per module, serving as the backend for all GlowNow clients (web and mobile).

## Architecture

The solution follows a **4-project-per-module** Clean Architecture:

```
src/
├── Api/GlowNow.Api/              # Composition root, DI, middleware
├── Core/
│   ├── GlowNow.SharedKernel/     # Domain primitives (Entity, Result, ValueObject)
│   └── GlowNow.Infrastructure.Core/  # Cross-cutting (behaviors, interfaces)
└── Modules/{Module}/
    ├── GlowNow.{Module}.Domain/       # Entities, value objects, events, errors
    ├── GlowNow.{Module}.Application/  # Commands, queries, handlers, validators
    ├── GlowNow.{Module}.Infrastructure/ # EF Core, repositories, external services
    └── GlowNow.{Module}.Api/          # MVC controllers, module DI registration
```

**Dependency rule:** Dependencies point inward. Domain has no external references. Application depends only on Domain + SharedKernel. Infrastructure implements Application interfaces.

## Modules

| Module | Status | Description |
|--------|--------|-------------|
| Identity | Complete | Auth, JWT, users, roles |
| Business | Complete | Tenant registration, settings, operating hours |
| Catalog | Complete | Services, categories, pricing |
| Team | Complete | Staff, shifts, availability, time-off |
| Clients | Scaffold | Client profiles, history |
| Booking | Scaffold | Availability calculation, appointments |
| Notifications | Scaffold | Email/SMS dispatch |

## Tech Stack

- **.NET 10** (SDK 10.0.100)
- **MVC Controllers** for API endpoints
- **EF Core 10** with PostgreSQL
- **MediatR** for CQRS
- **FluentValidation** for input validation
- **Warnings as errors** enforced via `Directory.Build.props`

## Development

```bash
# From the repo root
npx turbo dev --filter=api

# Or from this directory
dotnet run --project src/Api/GlowNow.Api
```

The API runs on [http://localhost:5249](http://localhost:5249).

## Scripts

```bash
dotnet build GlowNow.Api.sln                          # Build the solution
dotnet build GlowNow.Api.sln --configuration Release  # Release build
dotnet run --project src/Api/GlowNow.Api              # Run the API
dotnet format GlowNow.Api.sln --verify-no-changes     # Check formatting
```

## Key Endpoints

| Group | Base Path | Description |
|-------|-----------|-------------|
| Auth | `/api/v1/auth` | Register, login, refresh, logout |
| Business | `/api/v1/businesses` | Business CRUD, operating hours |
| Services | `/api/v1/services` | Service and category CRUD |
| Staff | `/api/v1/staff` | Staff profiles, schedules, time-off |

See `docs/ai/current-state.md` for the full endpoint list.
