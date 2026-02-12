# GlowNow API — Module Architecture

> Last updated: 2026-02-11

This document describes the 7 domain modules + 2 core packages in the GlowNow API.

---

## Architecture Overview

The API follows a **4-project-per-module** Clean Architecture pattern:

```
src/
├── Api/GlowNow.Api/              # Composition root (host)
├── Core/
│   ├── GlowNow.SharedKernel/     # Domain primitives
│   └── GlowNow.Infrastructure.Core/  # Cross-cutting concerns
└── Modules/{Module}/
    ├── GlowNow.{Module}.Domain/
    ├── GlowNow.{Module}.Application/
    ├── GlowNow.{Module}.Infrastructure/
    └── GlowNow.{Module}.Api/
```

### Project Responsibilities

| Project | Layer | Responsibility |
|---------|-------|---------------|
| `GlowNow.{Module}.Domain` | Domain | Entities, value objects, events, errors, domain services |
| `GlowNow.{Module}.Application` | Application | Commands, queries, handlers, validators, interfaces |
| `GlowNow.{Module}.Infrastructure` | Infrastructure | EF Core DbContext, repositories, external service implementations |
| `GlowNow.{Module}.Api` | Presentation | MVC controllers, module DI registration |

### Dependency Flow

```
Api → Infrastructure → Application → Domain → SharedKernel
```

---

## Core Packages

### GlowNow.SharedKernel

Foundation layer with domain primitives used by all modules.

**Contents:**
- `Entity<TId>` — Base class for entities with identity
- `AggregateRoot<TId>` — Base class for aggregate roots with domain events
- `ValueObject` — Base class for value objects
- `Result`, `Result<T>` — Result pattern for error handling
- `Error`, `ValidationError` — Error types
- `IDomainEvent` — Domain event interface
- `ITenantScoped` — Marker for multi-tenant entities
- **Value Objects:** `Email`, `PhoneNumber` (Ecuador +593 format)

**Dependencies:** None (foundation)

### GlowNow.Infrastructure.Core

Cross-cutting infrastructure concerns shared by all modules.

**Contents:**
- **CQRS Abstractions:** `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`
- **MediatR Behaviors:** `LoggingBehavior`, `ValidationBehavior`, `TransactionBehavior`, `PerformanceBehavior`
- **Application Interfaces:** `IUnitOfWork`, `IDateTimeProvider`, `ITenantProvider`, `ICurrentUserProvider`, `ITransactionManager`
- **Default Implementations:** `SystemDateTimeProvider`, `NoOpTransactionManager`, `HttpTenantProvider`, `CurrentUserProvider`

**Dependencies:** GlowNow.SharedKernel

---

## Domain Modules

### 1. Identity Module

**Responsibility:** Authentication, authorization, user management

**Status:** ✅ Complete

**What it does:**
- User registration and login (email/password via AWS Cognito)
- JWT token generation and validation (RS256)
- Refresh token management (rotating, single-use)
- Role management (Owner, Manager, Staff, Receptionist, Client)
- Business membership management

**Key entities:**
- `User` — Platform user with email, name, phone, Cognito ID
- `BusinessMembership` — User's role in a business
- `UserRole` enum — Permission levels

**CQRS handlers:**
- `RegisterBusinessCommand` — Creates Cognito user + local User + Business
- `LoginCommand` — Authenticates via Cognito
- `RefreshTokenCommand` — Token refresh
- `LogoutCommand` — Global sign-out
- `GetCurrentUserQuery` — Current user with memberships

**API endpoints:** `/api/v1/auth/*`

**Dependencies:** SharedKernel, Infrastructure.Core, Business (for IBusinessRepository)

---

### 2. Business Module

**Responsibility:** Tenant registration, business settings, operating hours

**Status:** ✅ Complete

**What it does:**
- Business registration with RUC validation (Ecuador-specific)
- Manage business profile (name, logo, description, address)
- Configure operating hours per day of week
- Business-level settings

**Key entities:**
- `Business` — Tenant entity with RUC, address, operating hours
- `Ruc` value object — 10-digit cédula or 13-digit RUC validation
- `OperatingHours` value object — Weekly schedule (JSONB)
- `TimeRange` value object — Opening/closing time pair

**CQRS handlers:**
- `SetOperatingHoursCommand` — Set weekly schedule
- `UpdateBusinessSettingsCommand` — Update name, description, logo
- `GetBusinessDetailsQuery` — Full business info
- `GetOperatingHoursQuery` — Weekly schedule

**API endpoints:** `/api/v1/businesses/*`

**Dependencies:** SharedKernel only

---

### 3. Catalog Module

**Responsibility:** Service offerings, categories, pricing

**Status:** ✅ Complete

**What it does:**
- Create and manage services (name, description, duration, price)
- Organize services into categories
- Set buffer time per service
- Soft delete support for history preservation

**Key entities:**
- `Service` — Service offering with duration, price, buffer time
- `ServiceCategory` — Groups related services
- `Duration` value object — Service time in minutes (5-480)
- `Money` value object — USD price with decimal precision

**CQRS handlers:**
- Full CRUD for Services and Categories
- `GetAllServicesQuery`, `GetServicesByCategoryQuery`

**API endpoints:** `/api/v1/services/*`, `/api/v1/services/categories/*`

**Dependencies:** SharedKernel only

---

### 4. Team Module

**Responsibility:** Staff management, schedules, availability

**Status:** ✅ Complete

**What it does:**
- Create and manage staff profiles
- Assign services to staff members
- Shift scheduling with weekly patterns
- Time-off requests and approvals
- Blocked time management (breaks, meetings)
- Staff availability calculation

**Key entities:**
- `StaffProfile` — Staff member with display name, schedule, status
- `StaffServiceAssignment` — Many-to-many staff ↔ services
- `BlockedTime` — Unavailable time ranges
- `TimeOff` — Vacation, sick leave, etc.
- `WeeklySchedule`, `WorkDay` value objects

**CQRS handlers:**
- Staff profile CRUD
- Service assignment/unassignment
- Time-off request/approve/reject/cancel
- Blocked time CRUD
- `GetStaffAvailabilityQuery`, `GetStaffScheduleQuery`

**API endpoints:** `/api/v1/staff/*`

**Dependencies:** SharedKernel, Identity (users), Catalog (services)

---

### 5. Clients Module

**Responsibility:** Client profiles, booking history

**Status:** 🟡 Scaffold only

**What it will do:**
- Create and manage client records (name, phone, email, notes)
- Search clients by phone/name
- Track client booking history
- Client preferences and notes
- No-show tracking and client flags

**Key entities (planned):**
- `Client` — Client profile
- `ClientNote` — Staff notes about client
- `ClientBookingHistory` — Past appointments

**Dependencies:** SharedKernel, Identity (optional for self-registration)

---

### 6. Booking Module

**Responsibility:** Availability calculation, appointments

**Status:** 🟡 Scaffold only

**What it will do:**
- **Availability calculation** (core algorithm):
  - Query staff shifts for selected date
  - Subtract blocked time ranges
  - Subtract existing appointments
  - Apply service duration + buffer time
  - Generate available time slots
- Create appointments (online, manual, walk-in)
- Appointment lifecycle (reschedule, cancel, complete, no-show)
- Double-booking prevention (optimistic locking)
- "Any Professional" assignment logic

**Key entities (planned):**
- `Appointment` — Booked slot with service, staff, client
- `AppointmentStatus` enum
- `TimeSlot` value object
- `AvailabilityWindow` value object

**Dependencies:** SharedKernel, Team, Catalog, Clients, Notifications

**Note:** This is the most complex module — the availability calculation is the heart of the business logic.

---

### 7. Notifications Module

**Responsibility:** Email and SMS dispatch

**Status:** 🟡 Scaffold only (logging implementations ready)

**What it will do:**
- Send booking confirmations (email + SMS)
- Send 24-hour reminders (SMS)
- Send cancellation notifications
- Notify staff of new bookings
- Template management (Spanish/English)
- Delivery tracking and retry logic

**Key interfaces:**
- `IEmailService` — Email dispatch
- `ISmsService` — SMS dispatch

**Current implementations:**
- `LoggingEmailService` — Logs emails for development
- `LoggingSmsService` — Logs SMS for development

**Dependencies:** SharedKernel only (listens to domain events from other modules)

---

## Module Dependencies (Acyclic)

```
                    ┌─────────────────────────────┐
                    │      SharedKernel           │
                    │  (Domain Primitives)        │
                    └─────────────────────────────┘
                                ▲
                                │
                    ┌───────────┴───────────┐
                    │  Infrastructure.Core  │
                    │  (Behaviors, Interfaces)│
                    └───────────────────────┘
                                ▲
         ┌──────────────────────┼──────────────────────┐
         │                      │                      │
    ┌────┴────┐           ┌────┴────┐           ┌────┴────┐
    │Identity │           │Business │           │ Catalog │
    └────┬────┘           └─────────┘           └────┬────┘
         │                                           │
         └───────────────────┬───────────────────────┘
                             │
                        ┌────┴────┐
                        │  Team   │
                        └────┬────┘
                             │
              ┌──────────────┼──────────────┐
              │              │              │
         ┌────┴────┐         │         ┌────┴────┐
         │ Clients │         │         │Notifica-│
         └────┬────┘         │         │ tions   │
              │              │         └─────────┘
              └──────────────┤              ▲
                             │              │ (events)
                        ┌────┴────┐         │
                        │ Booking │─────────┘
                        └─────────┘
```

**Dependency rules:**
- SharedKernel has no dependencies (foundation)
- Infrastructure.Core depends only on SharedKernel
- Identity, Business, Catalog depend only on Core packages
- Team depends on Identity (users) + Catalog (services)
- Clients depends on Identity (optional)
- Booking depends on Team (availability) + Catalog (services) + Clients
- Notifications depends only on Core (listens to events)

---

## Module Communication Patterns

### 1. Direct Service Calls (Synchronous)

When Booking needs Team data:

```csharp
// In Booking.Application
public class AvailabilityCalculationService
{
    private readonly IStaffAvailabilityService _staffAvailability; // From Team

    public async Task<List<TimeSlot>> CalculateAsync(
        Guid serviceId,
        Guid? staffId,
        DateOnly date)
    {
        var availableWindows = await _staffAvailability
            .GetAvailableWindowsAsync(staffId, date);

        // Apply service duration/buffer logic
        // Return available time slots
    }
}
```

### 2. Domain Events (Asynchronous)

When Booking creates an appointment, notify others:

```csharp
// In Booking.Domain
public class Appointment : AggregateRoot<Guid>
{
    public void Confirm()
    {
        Status = AppointmentStatus.Confirmed;
        RaiseDomainEvent(new AppointmentConfirmedEvent(
            Id, BusinessId, ClientId, StaffId, StartTime
        ));
    }
}

// In Notifications.Application - Event Handler
public class AppointmentConfirmedHandler
    : INotificationHandler<AppointmentConfirmedEvent>
{
    private readonly IEmailService _email;
    private readonly ISmsService _sms;

    public async Task Handle(AppointmentConfirmedEvent evt, CancellationToken ct)
    {
        await _email.SendConfirmationAsync(evt);
        await _sms.SendConfirmationAsync(evt);
    }
}
```

### 3. Shared Kernel (Value Objects)

All modules use value objects from SharedKernel:

```csharp
// In SharedKernel
public record Email
{
    private Email(string value) => Value = value;
    public string Value { get; }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !IsValidFormat(value))
            return Result.Failure<Email>(new Error("Email.Invalid", "Invalid email"));
        return Result.Success(new Email(value.ToLowerInvariant()));
    }
}

// Used in Identity, Business, Clients modules
```

---

## Multi-Tenancy Implementation

### EF Core Query Filters

Each module's DbContext applies tenant filtering:

```csharp
public class TeamDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StaffProfile>().HasQueryFilter(
            s => s.BusinessId == _tenantProvider.GetCurrentBusinessId());
    }
}
```

### Tenant Resolution (Middleware)

```csharp
public class TenantMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var businessId = context.Request.Headers["X-Business-Id"].FirstOrDefault();

        if (Guid.TryParse(businessId, out var id))
        {
            // Validate user has access to this business via membership
            _tenantProvider.SetCurrentBusiness(id);
        }

        await _next(context);
    }
}
```

---

## Adding a New Module

1. **Create 4 projects** in `src/Modules/{Module}/`:
   - `GlowNow.{Module}.Domain`
   - `GlowNow.{Module}.Application`
   - `GlowNow.{Module}.Infrastructure`
   - `GlowNow.{Module}.Api`

2. **Add project references:**
   - Domain → SharedKernel
   - Application → Domain, Infrastructure.Core
   - Infrastructure → Application
   - Api → Infrastructure

3. **Create module registration** in `{Module}Module.cs`:
   ```csharp
   public static class MyModule
   {
       public static IServiceCollection AddMyModule(this IServiceCollection services)
       {
           // Register Application services
           services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(
               typeof(MyModule).Assembly));

           // Register Infrastructure services
           services.AddMyModuleInfrastructure();

           return services;
       }
   }
   ```

4. **Register in host** `Program.cs`:
   ```csharp
   builder.Services.AddMyModule();
   ```

5. **Add MVC controllers** in `GlowNow.{Module}.Api/Controllers/`

6. **Create DbContext** with tenant-scoped query filters

7. **Generate EF Core migrations**:
   ```bash
   dotnet ef migrations add InitialCreate \
     --project src/Modules/{Module}/GlowNow.{Module}.Infrastructure \
     --startup-project src/Api/GlowNow.Api \
     --context {Module}DbContext
   ```
