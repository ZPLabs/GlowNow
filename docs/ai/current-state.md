# GlowNow — Current State

> Living document tracking the latest development status. Update this after each meaningful milestone.

**Last updated:** 2026-02-11
**Current branch:** `feature/login-implementation`
**Latest commit on main:** `ca44e2b` — Merge pull request #5 from ZPLabs/feature/team-module

---

## Project Phase

**Phase: Team Module Complete / Pre-MVP**

Major architecture refactor completed. The API has been restructured from a single-project-per-module layout to a 4-project-per-module architecture (Domain, Application, Infrastructure, Api). Minimal APIs have been converted to MVC Controllers. The Team module is now fully implemented with staff management and scheduling. EF Core migrations have been generated for Identity, Business, and Catalog modules. 88+ unit tests pass. The API is ready for local PostgreSQL integration testing.

---

## Recent Major Refactor (2026-02-10)

### 4-Project-Per-Module Architecture

**Commit:** `2c34bbd` — "refactor(api): restructure to 4-project-per-module architecture with MVC controllers"

#### Before (single .csproj per module):
```
src/Modules/GlowNow.Business/
├── Application/
├── Domain/
├── Infrastructure/
└── GlowNow.Business.csproj
```

#### After (4 .csproj per module):
```
src/Modules/Business/
├── GlowNow.Business.Domain/
├── GlowNow.Business.Application/
├── GlowNow.Business.Infrastructure/
└── GlowNow.Business.Api/
```

#### Key Changes:

1. **Shared layer split:**
   - `GlowNow.SharedKernel` — Domain primitives (Entity, AggregateRoot, ValueObject, Result, Error)
   - `GlowNow.Infrastructure.Core` — Cross-cutting concerns (behaviors, interfaces, providers)

2. **Minimal APIs → MVC Controllers:**
   - All endpoints converted from `app.MapGet/MapPost` to `[ApiController]` classes
   - Better OpenAPI/Swagger support and route grouping
   - Each module's Api project contains its own controllers

3. **Host project relocated:**
   - From `src/GlowNow.Api/` to `src/Api/GlowNow.Api/` (composition root)

4. **New project structure:**
   ```
   src/
   ├── Api/GlowNow.Api/              # Composition root (host)
   ├── Core/GlowNow.SharedKernel/    # Domain primitives
   ├── Core/GlowNow.Infrastructure.Core/  # Cross-cutting
   └── Modules/{Module}/             # Domain, Application, Infrastructure, Api
   ```

### Follow-up Fix (2026-02-11)

**Commit:** `1bdd389` — "fix(api): fix EF Core/Npgsql version mismatch and enhance Cognito auth"

- Updated Npgsql.EntityFrameworkCore.PostgreSQL to 10.0.0-preview.3 (matches EF Core)
- Added AccessKey/SecretKey support in CognitoSettings for local development
- Enhanced CognitoIdentityProvider with better error logging
- Generated initial EF Core migrations for Identity, Business, and Catalog modules

---

## What's Done

### Monorepo Scaffold (merged to `main`)

- Turborepo + npm workspaces with `apps/web`, `apps/mobile`, `apps/api`, and shared `packages/`
- Next.js 16 web app (React 19, CSS Modules) — default template, no custom pages
- Expo 54 mobile app — blank TypeScript template, no custom screens
- Shared packages: `@glownow/ui`, `@glownow/eslint-config`, `@glownow/typescript-config`
- .NET 10 API with 4-project-per-module modular monolith architecture
- 7 domain modules: Identity, Business, Catalog, Team, Clients, Booking, Notifications
- `GET /health` and `GET /health/ready` endpoints
- Docker infrastructure scripts
- ARCHITECTURE.md, PRD.md, GIT_GUIDE.md, CLAUDE.md context files

### Core Foundation (merged to `main`)

#### GlowNow.SharedKernel
- **Domain Primitives:** `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `ITenantScoped`, `IDomainEvent`
- **Result/Error Pattern:** `Result`, `Result<TValue>`, `Error` (sealed record), `ValidationError`
- **Value Objects:** `Email`, `PhoneNumber` (Ecuador +593 format)

#### GlowNow.Infrastructure.Core
- **CQRS Abstractions:** `ICommand`/`IQuery` with MediatR, handler interfaces
- **MediatR Pipeline Behaviors** (in order): Logging, Validation, Transaction, Performance
- **Application Interfaces:** `IUnitOfWork`, `IDateTimeProvider`, `ITenantProvider`, `ITransactionManager`, `ICurrentUserProvider`
- **Default Implementations:** `SystemDateTimeProvider`, `NoOpTransactionManager`, `HttpTenantProvider`
- **DI Registration:** `AddInfrastructureCoreServices(params Assembly[])`

### Identity Module (merged to `main`)

- **Domain Layer:**
  - `User` aggregate root — Email, FirstName, LastName, PhoneNumber?, CognitoUserId, timestamps, Memberships
  - `BusinessMembership` entity — UserId, BusinessId, Role, CreatedAtUtc
  - `UserRole` enum — Owner, Manager, Staff, Receptionist, Client
  - `UserRegisteredEvent` domain event
  - `IdentityErrors` — EmailAlreadyInUse, InvalidCredentials, UserNotFound, CognitoError, InvalidRefreshToken

- **Application Layer (CQRS):**
  - `RegisterBusinessCommand` — Creates Cognito user + local User + Business + BusinessMembership (Owner role)
  - `LoginCommand` — Authenticates via Cognito, looks up local User for memberships
  - `LogoutCommand` — Global sign-out via Cognito
  - `RefreshTokenCommand` — Token refresh via Cognito
  - `GetCurrentUserQuery` — Loads User + memberships with business names

- **Infrastructure Layer:**
  - `IdentityDbContext` with EF Core configurations
  - Repository implementations: `UserRepository`, `BusinessMembershipRepository`
  - `CognitoIdentityProvider` — AWS SDK adapter with AccessKey/SecretKey support

- **API Layer:**
  - `AuthController` — MVC controller for all auth endpoints

### Business Module (merged to `main`)

- **Domain Layer:**
  - `Business` aggregate root — Name, Ruc, Address, PhoneNumber?, Email, Description?, LogoUrl?, OperatingHours
  - `Ruc` value object — Validates 10-digit cedula or 13-digit RUC
  - `TimeRange`, `OperatingHours` value objects
  - `BusinessRegisteredEvent` domain event

- **Application Layer (CQRS):**
  - `SetOperatingHoursCommand`, `UpdateBusinessSettingsCommand`
  - `GetBusinessDetailsQuery`, `GetOperatingHoursQuery`

- **Infrastructure Layer:**
  - `BusinessDbContext` with JSONB column for operating hours
  - `BusinessRepository` implementation
  - Initial EF Core migration generated

- **API Layer:**
  - `BusinessesController` — MVC controller for business CRUD

### Catalog Module (merged to `main`)

- **Domain Layer:**
  - `Service` aggregate root — Name, Description?, Duration, Price, BufferTimeMinutes, CategoryId?, IsActive, DisplayOrder, IsDeleted
  - `ServiceCategory` aggregate root — Name, Description?, DisplayOrder, IsDeleted
  - `Duration`, `Money` value objects

- **Application Layer (CQRS):**
  - Full CRUD commands for Services and Categories
  - Query handlers for listing and filtering

- **Infrastructure Layer:**
  - `CatalogDbContext` with owned value objects
  - `ServiceRepository`, `ServiceCategoryRepository` implementations
  - Initial EF Core migration generated

- **API Layer:**
  - `ServicesController`, `ServiceCategoriesController` — MVC controllers

### Team Module (merged to `main`)

- **Domain Layer:**
  - `StaffProfile` aggregate root — UserId, BusinessId, DisplayName, Status, WeeklySchedule
  - `BlockedTime`, `TimeOff`, `StaffServiceAssignment` entities
  - `WeeklySchedule`, `WorkDay` value objects
  - `StaffStatus`, `TimeOffStatus`, `TimeOffType` enums
  - `TeamErrors`, domain events

- **Application Layer (CQRS):**
  - Staff profile CRUD commands
  - Time off request/approve/reject/cancel commands
  - Blocked time management
  - Service assignment commands
  - Schedule and availability queries

- **Infrastructure Layer:**
  - `TeamDbContext` with EF Core configurations
  - Repository implementations

- **API Layer:**
  - `StaffController` — comprehensive MVC controller with 20+ endpoints

### EF Core Migrations

| Module | Migration | Tables |
|--------|-----------|--------|
| Identity | `20260211003426_InitialCreate` | users, businesses, business_memberships |
| Business | `20260211003443_InitialCreate` | businesses (with JSONB operating_hours) |
| Catalog | `20260211003502_InitialCreate` | services, service_categories |

### Unit Tests — 88+ passing

| Area | Tests |
|------|-------|
| Shared (Entity, AggregateRoot, ValueObject, Result, Error) | 17 |
| Shared Value Objects (Email, PhoneNumber) | 12 |
| Business Value Objects (Ruc) | 7 |
| Business Entities (Business) | 4 |
| Identity Entities (User, BusinessMembership) | 8 |
| Identity Handlers | 14 |
| Identity Validators | 13 |
| Team Domain Layer | ~13 |
| **Total** | **88+** |

---

## What's Not Done

### Immediate Next Steps

1. **Start PostgreSQL** via Docker Compose
2. **Apply EF Core migrations** — `dotnet ef database update`
3. **Smoke test with real Cognito User Pool** — configure `appsettings.Development.json`
4. **Clients Module** — Client profiles and history
5. **Booking Module** — Core booking engine

### Not Started (by module)

| Module | Status | Dependencies |
|--------|--------|-------------|
| Identity | **Complete** | Shared, Business |
| Business | **Complete** | Shared only |
| Catalog | **Complete** | Shared only |
| Team | **Complete** | Identity, Catalog |
| Clients | Not started | Identity (optional) |
| Booking | Not started | Team, Catalog, Clients |
| Notifications | Scaffold only | Shared only (event-driven) |

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
- No design system beyond the default `@glownow/ui` components

---

## Architecture Decisions Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2026-01-20 | Modular monolith over microservices | MVP scale doesn't justify distributed complexity |
| 2026-02-07 | `Result<T> : Result` inheritance | Enables pipeline behavior constraints |
| 2026-02-08 | AWS Cognito for authentication | API-proxied flow, clients never interact with Cognito directly |
| 2026-02-08 | Local User shadow in PostgreSQL | Enables multi-tenancy joins without Cognito limitations |
| 2026-02-09 | OperatingHours as JSONB | Flexible weekly schedule storage |
| 2026-02-09 | Soft deletes for Services and Categories | Preserves booking history integrity |
| 2026-02-10 | **4-project-per-module architecture** | Better separation of concerns, cleaner dependency graph, easier testing |
| 2026-02-10 | **MVC Controllers over Minimal APIs** | Better OpenAPI support, route grouping, familiar patterns |
| 2026-02-10 | **SharedKernel/Infrastructure.Core split** | Domain primitives isolated from cross-cutting infrastructure |

---

## Project Structure

```
apps/api/
├── GlowNow.Api.sln
├── src/
│   ├── Api/
│   │   └── GlowNow.Api/              # Composition root (host)
│   ├── Core/
│   │   ├── GlowNow.SharedKernel/     # Domain primitives
│   │   └── GlowNow.Infrastructure.Core/  # Cross-cutting
│   └── Modules/
│       ├── Identity/
│       │   ├── GlowNow.Identity.Domain/
│       │   ├── GlowNow.Identity.Application/
│       │   ├── GlowNow.Identity.Infrastructure/
│       │   └── GlowNow.Identity.Api/
│       ├── Business/
│       │   └── ... (same 4-project layout)
│       ├── Catalog/
│       ├── Team/
│       ├── Clients/
│       ├── Booking/
│       └── Notifications/
└── tests/
    ├── GlowNow.UnitTests/
    ├── GlowNow.IntegrationTests/
    └── GlowNow.ApiTests/
```

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

### Staff (`/api/v1/staff`)
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | Required | Create staff profile |
| GET | `/` | Required | List all staff |
| GET | `/{id}` | Required | Get staff profile |
| PUT | `/{id}` | Required | Update staff profile |
| DELETE | `/{id}` | Required | Delete staff profile |
| POST | `/{id}/activate` | Required | Activate staff |
| POST | `/{id}/deactivate` | Required | Deactivate staff |
| PUT | `/{id}/schedule` | Required | Update schedule |
| GET | `/{id}/schedule` | Required | Get schedule |
| GET | `/{id}/availability` | Required | Get availability |
| GET | `/me/schedule` | Required | Get my schedule |
| POST | `/{id}/services` | Required | Assign service |
| DELETE | `/{id}/services/{serviceId}` | Required | Unassign service |
| GET | `/by-service/{serviceId}` | Required | Staff by service |
| POST | `/{id}/time-off` | Required | Request time off |
| GET | `/{id}/time-off` | Required | Get time off |
| POST | `/time-off/{id}/approve` | Required | Approve time off |
| POST | `/time-off/{id}/reject` | Required | Reject time off |
| POST | `/time-off/{id}/cancel` | Required | Cancel time off |
| POST | `/{id}/blocked-times` | Required | Create blocked time |
| GET | `/{id}/blocked-times` | Required | Get blocked times |
| DELETE | `/blocked-times/{id}` | Required | Delete blocked time |
