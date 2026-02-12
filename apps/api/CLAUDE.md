# CLAUDE.md — GlowNow API

> Rules and conventions for AI-assisted development. For detailed patterns, code examples, and data model see `docs/ARCHITECTURE.md`.

---

## Project Overview

GlowNow is a multi-tenant SaaS booking platform for Ecuador's beauty/wellness industry. The API is a **.NET 10 modular monolith** using Clean Architecture, CQRS with MediatR, and Domain-Driven Design.

**Stack:** .NET 10 · C# 14 · PostgreSQL · EF Core · MediatR · FluentValidation · Scalar (OpenAPI)

---

## Repository Structure

```
apps/api/
├── src/
│   ├── Api/
│   │   └── GlowNow.Api/              # Composition root — DI, middleware, MVC host
│   ├── Core/
│   │   ├── GlowNow.SharedKernel/     # Domain primitives — Entity, AggregateRoot, ValueObject, Result
│   │   └── GlowNow.Infrastructure.Core/  # Cross-cutting — behaviors, interfaces, providers
│   └── Modules/
│       ├── Identity/                  # Auth, JWT, users, roles
│       │   ├── GlowNow.Identity.Domain/
│       │   ├── GlowNow.Identity.Application/
│       │   ├── GlowNow.Identity.Infrastructure/
│       │   └── GlowNow.Identity.Api/
│       ├── Business/                  # Tenant registration, settings, operating hours
│       │   └── ... (same 4-project layout)
│       ├── Catalog/                   # Services, categories, pricing
│       ├── Team/                      # Staff, shifts, blocked time, availability
│       ├── Clients/                   # Client profiles, search, history
│       ├── Booking/                   # Availability calculation, appointments
│       └── Notifications/             # Email/SMS dispatch via domain events
├── tests/
│   ├── GlowNow.UnitTests/            # xUnit unit tests per module
│   ├── GlowNow.IntegrationTests/     # Testcontainers-based integration tests
│   └── GlowNow.ApiTests/             # End-to-end API tests
├── Directory.Build.props              # Shared .NET settings (net10.0, nullable, warnings-as-errors)
└── GlowNow.Api.sln
```

### 4-Project-Per-Module Architecture

Each module follows Clean Architecture with 4 separate projects:

| Project | Layer | Purpose |
|---------|-------|---------|
| `GlowNow.{Module}.Domain` | Domain | Entities, value objects, events, errors |
| `GlowNow.{Module}.Application` | Application | Commands, queries, handlers, validators, interfaces |
| `GlowNow.{Module}.Infrastructure` | Infrastructure | EF Core, repositories, external services |
| `GlowNow.{Module}.Api` | Presentation | MVC controllers, module DI registration |

**Dependency flow:** Api → Infrastructure → Application → Domain (and SharedKernel)

---

## Module Dependencies

Dependencies are **acyclic** — never create circular references between modules.

```
Booking → Team, Catalog, Clients
Team → Identity, Catalog
Clients → Identity (optional)
Identity, Business, Catalog → Shared only
Notifications → Shared only (listens to domain events)
Shared → nothing (foundation)
```

---

## Architecture (summary)

- **Modular monolith:** Each module is a self-contained vertical slice with Domain, Application, and Infrastructure layers.
- **Clean Architecture:** Dependencies point inward. Never reference Infrastructure from Domain.
- **CQRS:** Every use case is a Command (write) or Query (read) dispatched through MediatR.
- **DDD:** Rich domain models with behavior. Entities protect invariants. Aggregate roots are persistence boundaries.
- **Inter-module communication:** Direct service interfaces (sync) or domain events (async).

---

## Coding Conventions

### General

- **Language:** C# 14 with nullable reference types enabled globally.
- **Target:** `net10.0` for all projects.
- **Warnings as errors:** Enforced via `Directory.Build.props`. Zero warnings allowed.
- **Readability first:** Favor clarity over cleverness.
- **Comments:** Explain *why*, not *what*. XML doc comments on all public APIs.

### Naming

| Element | Convention | Example |
|---------|-----------|---------|
| Class/Record | PascalCase | `CreateAppointmentCommand` |
| Interface | `I` + PascalCase | `IAppointmentRepository` |
| Method | PascalCase, verb-first | `CalculateAvailability()` |
| Private field | `_camelCase` | `_appointmentRepository` |
| Parameter | camelCase | `serviceId` |
| Constant | PascalCase | `MaxRetryCount` |
| Enum values | PascalCase | `AppointmentStatus.Confirmed` |
| File name | Matches type name | `CreateAppointmentCommand.cs` |

### File Organization (per module)

Each module has 4 separate projects:

```
Modules/{Module}/
├── GlowNow.{Module}.Domain/
│   ├── Entities/              # Aggregate roots and entities
│   ├── ValueObjects/          # Module-specific value objects
│   ├── Events/                # Domain events
│   ├── Enums/                 # Domain enumerations
│   ├── Errors/                # Domain error definitions
│   └── Services/              # Domain services (pure logic)
├── GlowNow.{Module}.Application/
│   ├── Commands/              # One folder per command (Command + Handler + Validator)
│   ├── Queries/               # One folder per query (Query + Handler + Response)
│   ├── Interfaces/            # Port interfaces (repositories, external services)
│   ├── Mappings/              # Entity ↔ Response mappings
│   └── EventHandlers/         # Handlers for domain events from other modules
├── GlowNow.{Module}.Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/    # EF Core entity configurations (Fluent API)
│   │   ├── Repositories/      # Repository implementations
│   │   └── Migrations/        # EF Core migrations (module-specific DbContext)
│   ├── Services/              # External service implementations
│   └── DependencyInjection.cs # Infrastructure DI registration
└── GlowNow.{Module}.Api/
    ├── Controllers/           # MVC controllers for this module
    ├── Infrastructure/        # API helpers (ResultExtensions, ApiResponse)
    └── {Module}Module.cs      # Module DI registration entry point
```

### Command/Query Rules

- One file per command/query, one file per handler, one file per validator.
- Group related command/query + handler + validator in a folder.
- Commands return `Result<T>` or `Result` (never throw for business rule violations).
- Queries return DTOs/response records, never domain entities.
- Validators use FluentValidation and run via MediatR pipeline behavior.

### Result Pattern

- Use `Result<T>` / `Result` for all Application layer returns.
- Never throw exceptions for expected failures.
- Define errors per module in `Domain/Errors/` as static `Error` instances.

### Validation

- **FluentValidation** for all command/query input validation.
- Validation runs automatically via MediatR `ValidationBehavior` pipeline.
- Domain invariants are enforced **inside entities** (guard clauses), not in validators.

---

## Multi-Tenancy

- Every tenant-scoped entity has a `BusinessId` property.
- EF Core global query filters apply `WHERE business_id = @current` automatically.
- Tenant resolved from `X-Business-Id` HTTP header via middleware.
- Middleware validates the authenticated user has access to the requested business.
- **Never** bypass tenant filters without explicit `IgnoreQueryFilters()` and a documented reason.

---

## API Conventions

- **MVC Controllers** — Each module has its own controllers in `GlowNow.{Module}.Api/Controllers/`.
- All endpoints prefixed with `/api/v1/`. URL-based versioning.
- Never remove v1 endpoints without a deprecation period.
- All endpoints documented via Scalar with XML doc comments and `[ProducesResponseType]` attributes.

### Response Format

Success: `{ "data": { ... }, "meta": { "timestamp": "...", "requestId": "..." } }`
Error: `{ "error": { "code": "...", "message": "...", "details": [...] } }`

### HTTP Status Codes

| Code | Usage |
|------|-------|
| 200 | Success (GET, PUT, PATCH) |
| 201 | Created (POST) — include `Location` header |
| 204 | No content (DELETE) |
| 400 | Validation error |
| 401 | Unauthenticated |
| 403 | Forbidden (wrong role / wrong tenant) |
| 404 | Resource not found |
| 409 | Conflict (double booking, duplicate) |
| 429 | Rate limited |
| 500 | Internal server error |

---

## Testing

| Project | Framework | What to test |
|---------|-----------|-------------|
| `tests/GlowNow.UnitTests/` | xUnit + NSubstitute | Domain entities, value objects, handlers, validators, domain services |
| `tests/GlowNow.IntegrationTests/` | xUnit + Testcontainers | Repositories, EF Core configs, multi-tenancy filters, query correctness |
| `tests/GlowNow.ApiTests/` | xUnit + WebApplicationFactory | Full HTTP request/response, auth, status codes, response shapes |

### Conventions

- **Naming:** `MethodName_Should_ExpectedBehavior_When_Condition`
- **Arrange-Act-Assert** in every test.
- **One assertion per test** (prefer multiple small tests over one large test).
- Domain entities and value objects must have comprehensive unit tests.
- Every command handler needs tests for success and failure paths.
- Availability calculation must have exhaustive edge case tests.
- Multi-tenancy isolation must be verified in integration tests.

---

## Cross-Cutting Patterns

- **Repository:** One interface per aggregate root in `Application/Interfaces/`, implementation in `Infrastructure/`. Returns domain entities, never DTOs. Use `IUnitOfWork` for transactions.
- **Domain Events:** Raised via `AddDomainEvent()`, dispatched after `SaveChangesAsync()` (outbox pattern). Notifications module listens — no direct coupling.
- **Logging:** `ILogger<T>` with structured properties and correlation IDs. Never `Console.WriteLine`.
- **Health Checks:** `GET /health` (liveness), `GET /health/ready` (readiness).
- **Rate Limiting:** Per-IP and per-user token bucket. Stricter on public endpoints. `429` with `Retry-After`.
- **Resilience:** Polly circuit breaker + retry with exponential backoff on external calls (Twilio, SendGrid). If SMS fails, log and don't block booking.
- **Security:** JWT RS256, policy-based auth, FluentValidation, CORS whitelist, HSTS, audit logging on all writes.
- **Migrations:** EF Core, version-controlled. Auto on dev, manual on staging/prod. Never edit existing migrations.

---

## MediatR Pipeline Behaviors

Register in this order:

1. **LoggingBehavior** — Log every request/response with correlation ID.
2. **ValidationBehavior** — Run FluentValidation, return errors before handler executes.
3. **TransactionBehavior** — Wrap commands in a DB transaction (queries are read-only).
4. **PerformanceBehavior** — Log slow requests (> 500ms warning, > 1000ms error).

---

## Ecuador-Specific Rules

- **RUC validation:** 13 digits for businesses, 10 for cedula. Validate check digit algorithm.
- **Phone format:** `+593 9X XXX XXXX` for mobile.
- **Currency:** USD — use `decimal` for all money values, never `float`/`double`.
- **Address:** Support Cuenca parishes (El Sagrario, San Sebastian, etc.).
- **Language:** Spanish primary, English secondary. All user-facing strings must be localizable.
- **Timezone:** `America/Guayaquil` (UTC-5, no DST). Store datetimes in UTC, convert for display.

---

## Common Mistakes to Avoid

- Returning domain entities from API endpoints (use response DTOs).
- Throwing exceptions for business rule violations (use Result pattern).
- Putting business logic in handlers (belongs in domain entities/services).
- Circular dependencies between modules.
- Bypassing multi-tenancy filters without explicit justification.
- Using `float`/`double` for money (use `decimal`).
- Hardcoding strings — use constants or resource files.
- Creating anemic domain models (entities must have behavior).
- Skipping tests for "simple" code.
- Direct module-to-module infrastructure calls (use application-layer interfaces).

---

## Git Conventions

- **Branch:** `feat/`, `fix/`, `chore/`, `refactor/`, `docs/`, `test/` + short description.
- **Commits:** Conventional commits — `feat(api): add service catalog endpoints`.
- **Scope:** `api`, `identity`, `booking`, `catalog`, `team`, `clients`, `notifications`, `shared`, `infra`.
- **PRs:** Squash-merge into `main`. PR title = conventional commit format.
- See `docs/GIT_GUIDE.md` for full details.

---

## Quick Reference: New Feature Checklist

1. **Domain first:** Define/update entities, value objects, events, domain errors in `GlowNow.{Module}.Domain/`.
2. **Application layer:** Create Command/Query + Handler + Validator + Response DTO in `GlowNow.{Module}.Application/`.
3. **Infrastructure:** Implement repository, EF Core configuration, migration in `GlowNow.{Module}.Infrastructure/`.
4. **API controller:** Add MVC controller action in `GlowNow.{Module}.Api/Controllers/`.
5. **Tests:** Unit tests for domain + handler, integration for repository, API tests for endpoint.
6. **OpenAPI:** Ensure endpoint is documented in Scalar with proper attributes.
