# GlowNow — Current State

> Living document tracking the latest development status. Update this after each meaningful milestone.

**Last updated:** 2026-02-09
**Current branch:** `feature/infrastructure-setup`
**Latest commit on main:** `6aaacea` — Merge pull request #3 from ZPLabs/feat(api)/identity-module-implementation

---

## Project Phase

**Phase: Business & Catalog Modules / Pre-MVP**

The Identity module is merged to `main`. The Business module is now fully implemented with operating hours, settings management, and CRUD endpoints. The Catalog module is fully implemented with services and categories. EF Core migration for the new schema has been generated. 88 unit tests pass. The API is ready for local PostgreSQL integration testing.

---

## What's Done

### Monorepo Scaffold (merged to `main`)

- Turborepo + npm workspaces with `apps/web`, `apps/mobile`, `apps/api`, and shared `packages/`
- Next.js 16 web app (React 19, CSS Modules) — default template, no custom pages
- Expo 54 mobile app — blank TypeScript template, no custom screens
- Shared packages: `@glownow/ui`, `@glownow/eslint-config`, `@glownow/typescript-config`
- .NET 10 API restructured as modular monolith with Clean Architecture
- 7 domain modules scaffolded: Identity, Business, Catalog, Team, Clients, Booking, Notifications
- `GET /health` and `GET /health/ready` endpoints
- Docker infrastructure scripts
- ARCHITECTURE.md, PRD.md, GIT_GUIDE.md, CLAUDE.md context files

### GlowNow.Shared Foundation (merged to `main`)

- **Domain Primitives:** `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `ITenantScoped`, `IDomainEvent`
- **Result/Error Pattern:** `Result`, `Result<TValue>`, `Error` (sealed record), `ValidationError`
- **CQRS Abstractions:** `ICommand`/`IQuery` with MediatR, `IBaseCommand` marker, handler interfaces
- **MediatR Pipeline Behaviors** (in order): Logging, Validation, Transaction, Performance
- **Application Interfaces:** `IUnitOfWork`, `IDateTimeProvider`, `ITenantProvider`, `ITransactionManager`
- **Default Implementations:** `SystemDateTimeProvider` (singleton), `NoOpTransactionManager` (scoped, replaceable via `TryAddScoped`)
- **DI Registration:** `AddSharedServices(params Assembly[])` with MediatR assembly scanning, FluentValidation, behavior pipeline

### Shared Infrastructure (merged to `main`)

- **EF Core Integration:** `AppDbContext` with multi-module entity configuration scanning, `EfCoreTransactionManager` (replaces NoOp)
- **Value Objects:** `Email` (format validation, lowercase normalization, max 256 chars), `PhoneNumber` (Ecuador +593 format, normalizes spaces/dashes/local formats)
- **Cross-Cutting Services:** `ICurrentUserProvider` (scoped, set by middleware), `HttpTenantProvider` (reads `X-Business-Id` header)
- **DI Registration:** `AddSharedInfrastructure(IConfiguration, params Assembly[])` registers DbContext, UnitOfWork, TransactionManager

### Identity Module (merged to `main`)

- **Domain Layer:**
  - `User` aggregate root — Email, FirstName, LastName, PhoneNumber?, CognitoUserId, timestamps, Memberships collection
  - `BusinessMembership` entity — UserId, BusinessId, Role, CreatedAtUtc
  - `UserRole` enum — Owner, Manager, Staff, Receptionist, Client
  - `UserRegisteredEvent` domain event
  - `IdentityErrors` — EmailAlreadyInUse, InvalidCredentials, UserNotFound, CognitoError, InvalidRefreshToken

- **Application Layer (CQRS):**
  - `RegisterBusinessCommand` + Handler + Validator — Creates Cognito user + local User + Business + BusinessMembership (Owner role). Compensating Cognito delete on DB failure.
  - `LoginCommand` + Handler + Validator — Authenticates via Cognito, looks up local User for memberships
  - `LogoutCommand` + Handler + Validator — Global sign-out via Cognito
  - `RefreshTokenCommand` + Handler + Validator — Token refresh via Cognito
  - `GetCurrentUserQuery` + Handler — Loads User + memberships with business names

- **Application Interfaces:**
  - `ICognitoIdentityProvider` — RegisterUser, Login, RefreshToken, GlobalSignOut, DeleteUser
  - `IUserRepository`, `IBusinessMembershipRepository`
  - `AuthTokens` record, `CognitoSettings` options class

- **Infrastructure Layer:**
  - EF Core configurations for User and BusinessMembership (Fluent API, value object mapping via OwnsOne)
  - Repository implementations: UserRepository, BusinessMembershipRepository
  - `CognitoIdentityProvider` — AWS SDK adapter mapping Cognito exceptions to domain errors

### Business Module (`feature/infrastructure-setup`, not yet merged)

- **Domain Layer:**
  - `Business` aggregate root — Name, Ruc, Address, PhoneNumber?, Email, Description?, LogoUrl?, OperatingHours, BusinessId (= own Id for ITenantScoped)
  - `Ruc` value object — Validates 10-digit cedula or 13-digit RUC, province code 01-24 or 30
  - `TimeRange` value object — Opening/closing time pair with validation
  - `OperatingHours` value object — Weekly schedule (Dict<DayOfWeek, TimeRange?>) with JSON serialization
  - `BusinessRegisteredEvent` domain event
  - `BusinessErrors` — DuplicateRuc, BusinessNotFound, InvalidRuc, InvalidTimeRange, InvalidOperatingHours, InvalidBusinessName, InvalidLogoUrl

- **Application Layer (CQRS):**
  - `SetOperatingHoursCommand` + Handler + Validator — Set weekly business hours
  - `UpdateBusinessSettingsCommand` + Handler + Validator — Update name, description, logo URL
  - `GetBusinessDetailsQuery` + Handler — Full business info including operating hours
  - `GetOperatingHoursQuery` + Handler — Weekly schedule only

- **Application Interfaces:**
  - `IBusinessRepository` — GetByIdAsync, ExistsByRucAsync, Add, Update

- **Infrastructure Layer:**
  - `BusinessConfiguration` — EF Core with JSONB column for operating hours, value object column mapping
  - `BusinessRepository` implementation
  - `BusinessModule` DI registration

- **API Endpoints:**
  - `GET /api/v1/businesses/{id}` — Get business details
  - `GET /api/v1/businesses/{id}/operating-hours` — Get operating hours
  - `PUT /api/v1/businesses/{id}/operating-hours` — Set operating hours
  - `PUT /api/v1/businesses/{id}/settings` — Update settings

### Catalog Module (`feature/infrastructure-setup`, not yet merged)

- **Domain Layer:**
  - `Service` aggregate root — Name, Description?, Duration, Price, BufferTimeMinutes, CategoryId?, IsActive, DisplayOrder, IsDeleted (soft delete)
  - `ServiceCategory` aggregate root — Name, Description?, DisplayOrder, IsDeleted (soft delete)
  - `Duration` value object — Service time in minutes (5-480 range)
  - `Money` value object — USD price with decimal precision (0-999,999.99)
  - `ServiceCreatedEvent`, `ServiceUpdatedEvent`, `ServiceCategoryCreatedEvent`, `ServiceCategoryUpdatedEvent` domain events
  - `CatalogErrors` — ServiceNotFound, CategoryNotFound, DuplicateServiceName, DuplicateCategoryName, InvalidDuration, InvalidPrice, InvalidServiceName, InvalidCategoryName, CategoryHasServices, ServiceIsActive

- **Application Layer (CQRS):**
  - `CreateServiceCategoryCommand` + Handler + Validator
  - `UpdateServiceCategoryCommand` + Handler + Validator
  - `DeleteServiceCategoryCommand` + Handler + Validator (soft delete, checks for services)
  - `CreateServiceCommand` + Handler + Validator
  - `UpdateServiceCommand` + Handler + Validator
  - `DeleteServiceCommand` + Handler + Validator (deactivates then soft deletes)
  - `GetAllCategoriesQuery` + Handler — List categories for business
  - `GetAllServicesQuery` + Handler — List services for business
  - `GetServiceQuery` + Handler — Get single service
  - `GetServicesByCategoryQuery` + Handler — Filter by category

- **Application Interfaces:**
  - `IServiceRepository` — GetByIdAsync, GetAllByBusinessIdAsync, GetByCategoryIdAsync, ExistsByNameAsync, Add, Update, Remove
  - `IServiceCategoryRepository` — GetByIdAsync, GetAllByBusinessIdAsync, ExistsByNameAsync, HasServicesAsync, Add, Update, Remove

- **Infrastructure Layer:**
  - `ServiceConfiguration` — EF Core with owned value objects (Duration, Money), soft-delete filter, unique name constraint
  - `ServiceCategoryConfiguration` — EF Core with soft-delete filter, unique name constraint
  - `ServiceRepository`, `ServiceCategoryRepository` implementations
  - `CatalogModule` DI registration

- **API Endpoints:**
  - `POST /api/v1/services/categories` — Create category
  - `GET /api/v1/services/categories?businessId=` — List categories
  - `PUT /api/v1/services/categories/{id}` — Update category
  - `DELETE /api/v1/services/categories/{id}` — Soft-delete category
  - `POST /api/v1/services` — Create service
  - `GET /api/v1/services?businessId=` — List services
  - `GET /api/v1/services/{id}` — Get service
  - `GET /api/v1/services/by-category/{categoryId}` — Services by category
  - `PUT /api/v1/services/{id}` — Update service
  - `DELETE /api/v1/services/{id}` — Soft-delete service

### API Host Wiring (merged to `main` + extended on feature branch)

- JWT Bearer authentication (Cognito JWKS endpoint)
- `CurrentUserMiddleware` — reads JWT claims, sets `ICurrentUserProvider`
- `TenantMiddleware` — reads `X-Business-Id` header, validates membership
- `AuthEndpoints` — POST register/login/refresh/logout, GET me
- `BusinessEndpoints` — GET business, GET/PUT operating-hours, PUT settings
- `ServiceCategoryEndpoints` — Full CRUD
- `ServiceEndpoints` — Full CRUD + by-category filter
- `ResultExtensions` + `ApiResponse` — standard response envelope
- Cognito + connection string configuration in appsettings

### EF Core Migrations

| Migration | Tables | Description |
|-----------|--------|-------------|
| `InitialCreate` | users, businesses, business_memberships | Identity + Business entities |
| `AddBusinessAndCatalogModules` | services, service_categories | + business columns (description, logo_url, operating_hours as JSONB) |

### Unit Tests — 88 passing (0 failures, 0 warnings)

| Area | Test Files | Tests |
|------|-----------|-------|
| Shared (Entity, AggregateRoot, ValueObject, Result, Error) | 5 | 17 |
| Shared Value Objects (Email, PhoneNumber) | 2 | 12 |
| Business Value Objects (Ruc) | 1 | 7 |
| Business Entities (Business) | 1 | 4 |
| Identity Entities (User, BusinessMembership) | 2 | 8 |
| Identity Handlers (RegisterBusiness, Login, Logout, RefreshToken, GetCurrentUser) | 5 | 14 |
| Identity Validators (RegisterBusiness, Login, Logout, RefreshToken) | 4 | 13 |
| **Subtotals** | **20** | **75** |
| Shared foundation (pre-existing) | — | 13 |
| **Total** | **20** | **88** |

### NuGet Dependencies

| Package | Version | Project |
|---------|---------|---------|
| MediatR | 12.4.1 | Shared |
| FluentValidation | 11.11.0 | Shared |
| FluentValidation.DependencyInjectionExtensions | 11.11.0 | Shared |
| Microsoft.Extensions.Logging.Abstractions | 10.0.0-preview.3 | Shared |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.0-preview.3 | Shared, Business, Catalog |
| Microsoft.EntityFrameworkCore | 10.0.0-preview.3 | Shared |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.0-preview.3 | Shared |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.0-preview.3 | Shared |
| AWSSDK.CognitoIdentityProvider | 3.7.404.2 | Identity |
| Microsoft.Extensions.Options.ConfigurationExtensions | 10.0.0-preview.3 | Identity |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.0-preview.3 | Api |
| Microsoft.EntityFrameworkCore.Design | 10.0.0-preview.3 | Api |
| xUnit | 2.9.3 | UnitTests |
| NSubstitute | 5.3.0 | UnitTests |
| FluentAssertions | 8.2.0 | UnitTests |

---

## What's Not Done

### Immediate Next Steps

1. **Merge `feature/infrastructure-setup` to `main`** via PR
2. **Start PostgreSQL** via Docker Compose
3. **Apply EF Core migrations** — `dotnet ef database update`
4. **Smoke test with real Cognito User Pool** — requires configuring `appsettings.Development.json` with real pool credentials
5. **Team Module** — Staff management, shifts, availability
6. **Clients Module** — Client profiles and history

### Not Started (by module)

| Module | Status | Dependencies |
|--------|--------|-------------|
| Identity | **Complete** (domain + app + infra + tests) | Shared, Business |
| Business | **Complete** (domain + app + infra + endpoints) | Shared only |
| Catalog | **Complete** (domain + app + infra + endpoints) | Shared only |
| Team | Not started | Identity, Catalog |
| Clients | Not started | Identity (optional) |
| Booking | Not started | Team, Catalog, Clients |
| Notifications | Not started | Shared only (event-driven) |

### Infrastructure / Cross-Cutting Not Started

- PostgreSQL database setup (local Docker Compose) — config exists, not applied
- Global exception handling middleware
- Correlation ID middleware
- CORS configuration
- Rate limiting
- Structured logging (Serilog or similar)
- CI/CD pipeline
- AWS infrastructure (Terraform)

### Frontend Not Started

- No custom pages or components in the web app
- No custom screens in the mobile app
- No API client / SDK generation
- No design system beyond the default `@glownow/ui` components (button, card, code)

---

## Architecture Decisions Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-01-20 | Modular monolith over microservices | MVP scale doesn't justify distributed complexity; can extract modules later |
| 2026-01-20 | Single .csproj per module (not split Domain/Application/Infrastructure) | Keeps it simple at MVP; folder-based separation within each module |
| 2026-02-07 | `Result<T> : Result` inheritance | Enables `where TResponse : Result` constraint in pipeline behaviors for both Result and Result<T> |
| 2026-02-07 | `NoOpTransactionManager` as default | Allows Shared to compile and run without EF Core; Infrastructure overrides via `TryAddScoped` |
| 2026-02-07 | `RaiseDomainEvent` naming (vs `AddDomainEvent`) | Clearer intent — aggregates "raise" events, they don't just "add" them |
| 2026-02-08 | AWS Cognito for authentication | API-proxied `ADMIN_NO_SRP_AUTH` flow — server holds Cognito credentials, clients never interact with Cognito directly |
| 2026-02-08 | Local User shadow in PostgreSQL | Enables multi-tenancy joins, audit logging, role management without Cognito custom attributes limitations |
| 2026-02-08 | Compensating Cognito delete on DB failure | If local DB save fails after Cognito user creation, delete the Cognito user to avoid orphans |
| 2026-02-08 | Shared value objects (Email, PhoneNumber) | Reused across Identity, Business, Clients, Team modules |
| 2026-02-08 | Ruc in Business module | Ecuador-specific, only relevant to Business domain |
| 2026-02-09 | `IBusinessRepository` consolidated in Business module | Identity now depends on Business module's repository interface, avoiding duplication |
| 2026-02-09 | OperatingHours as JSONB | Flexible weekly schedule storage; PostgreSQL JSONB enables querying if needed |
| 2026-02-09 | Soft deletes for Services and Categories | Preserves booking history integrity; IsDeleted flag with query filters |
| 2026-02-09 | Duration and Money as owned value objects | EF Core OwnsOne() for type safety while keeping flat table structure |
| 2026-02-09 | USD currency hardcoded in Money | Ecuador uses USD; no multi-currency complexity needed for MVP |

---

## Open Questions

- **API versioning middleware:** Use `Asp.Versioning.Http` or manual route prefixes?
- **Outbox pattern implementation:** Use a library (e.g., MassTransit outbox) or hand-roll with EF Core interceptors?
- **Cognito User Pool setup:** Need to configure a real pool for local development testing.
- **Multi-tenancy query filters:** Currently using manual `!IsDeleted` in queries; should migrate to global EF Core query filters with tenant scoping.

---

## File Counts

| Area | Source Files | Test Files | Tests |
|------|-------------|------------|-------|
| GlowNow.Shared | 35 | — | — |
| GlowNow.Api | 9 | — | — |
| GlowNow.Identity | 35 | — | — |
| GlowNow.Business | 22 | — | — |
| GlowNow.Catalog | 32 | — | — |
| Modules (4 empty shells) | 4 | — | — |
| GlowNow.UnitTests (GlobalUsings) | — | 1 | — |
| GlowNow.UnitTests/Shared | — | 7 | 30 |
| GlowNow.UnitTests/Identity | — | 11 | 35 |
| GlowNow.UnitTests/Business | — | 2 | 11 |
| **Total .NET** | **137** | **21** | **88** |

---

## API Endpoints Summary

### Authentication (`/api/v1/auth`)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/register` | Anonymous | Register business + owner |
| POST | `/login` | Anonymous | Login, get tokens |
| POST | `/refresh` | Anonymous | Refresh access token |
| POST | `/logout` | Required | Global sign-out |
| GET | `/me` | Required | Get current user + memberships |

### Business (`/api/v1/businesses`)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/{id}` | Required | Get business details |
| GET | `/{id}/operating-hours` | Required | Get weekly schedule |
| PUT | `/{id}/operating-hours` | Required | Set weekly schedule |
| PUT | `/{id}/settings` | Required | Update name, description, logo |

### Service Categories (`/api/v1/services/categories`)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | Required | Create category |
| GET | `/?businessId=` | Required | List categories |
| PUT | `/{id}` | Required | Update category |
| DELETE | `/{id}` | Required | Soft-delete category |

### Services (`/api/v1/services`)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | Required | Create service |
| GET | `/?businessId=` | Required | List services |
| GET | `/{id}` | Required | Get service |
| GET | `/by-category/{categoryId}` | Required | Services by category |
| PUT | `/{id}` | Required | Update service |
| DELETE | `/{id}` | Required | Soft-delete service |
