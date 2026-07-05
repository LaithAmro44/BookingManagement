# Booking Management Service

A focused booking-management application built for the **Backend Engineering Take-Home Assignment**. It provides a .NET 10 Web API and a small frontend for managing bookings of shared resources such as meeting rooms or equipment.

The application supports creating a booking, preventing overlapping bookings for the same resource, retrieving bookings with date-range filtering and paging, and cancelling bookings. It also includes unit tests for the core business rules and implements the **Concurrency** extension task.

---

## Features

- Create a booking for a resource and a user.
- Prevent active bookings from overlapping for the same resource.
- Allow back-to-back bookings, such as `09:00–10:00` followed by `10:00–11:00`.
- Retrieve bookings for a resource with UTC date-range filtering, paging, and optional cancelled-booking visibility.
- Cancel a booking using soft cancellation.
- List seeded resources and users for frontend dropdowns.
- Protect booking creation from same-resource race conditions.
- Provide Swagger/OpenAPI documentation.
- Cover the core booking logic with unit tests.

---

## Technology Stack

| Area | Technology |
|---|---|
| Backend | .NET 10 / ASP.NET Core Web API |
| Database | SQL Server |
| Data access | Entity Framework Core |
| Frontend | Angular |
| Tests | xUnit |
| API documentation | Swagger / OpenAPI |

---

## Architecture

The solution uses a lightweight Clean Architecture structure. The goal is to keep business rules independent from HTTP, Entity Framework Core, and SQL Server details while avoiding unnecessary complexity.

```text
src/
├── BookingManagement.Domain
│   ├── Entities
│   └── Enums
│
├── BookingManagement.Application
│   ├── Abstractions
│   ├── Bookings
│   └── Common
│
├── BookingManagement.Infrastructure
│   ├── Persistence
│   ├── Repositories
│   └── Concurrency
│
└── BookingManagement.Api
    ├── Controllers
    ├── Middleware
    └── Program.cs

tests/
└── BookingManagement.Application.Tests

frontend/
└── booking-management-ui
```

### Layer responsibilities

| Layer | Responsibility |
|---|---|
| `Domain` | Booking, Resource, BookingUser, BookingStatus, and domain rules. It has no dependency on EF Core or HTTP. |
| `Application` | Use cases, DTOs, repository abstractions, validation, booking orchestration, and application exceptions. |
| `Infrastructure` | EF Core `DbContext`, SQL Server mappings, repositories, migrations, seeded data, and the concurrency lock implementation. |
| `Api` | Controllers, dependency injection, exception-to-HTTP mapping, Swagger, and API configuration. |
| `Tests` | Unit tests for application and domain behavior without requiring a real database. |
| `Frontend` | Angular UI that consumes the API only. |

---

## Domain Model

### Booking

| Field | Description |
|---|---|
| `Id` | Unique `Guid` identifier. |
| `ResourceId` | Identifier of the booked resource. |
| `UserId` | Identifier of the user who created the booking. |
| `StartDateTimeUtc` | Start time, stored in UTC. |
| `EndDateTimeUtc` | End time, stored in UTC. |
| `Status` | `Active` or `Cancelled`. |
| `CreatedAtUtc` | Creation timestamp in UTC. |
| `CancelledAtUtc` | Cancellation timestamp in UTC, when applicable. |

### Resource and BookingUser

`Resource` and `BookingUser` are intentionally small, read-only reference entities. They are seeded with sample data and are used by the frontend as dropdown values.

User authentication and administration of resources/users are deliberately out of scope because the assignment focuses on booking lifecycle, correctness, and concurrency.

---

## Key Business Decisions

| Topic | Decision |
|---|---|
| Time representation | The API accepts ISO-8601 timestamps and normalizes them to UTC using `DateTimeOffset`. |
| Valid time range | A booking must have `StartDateTimeUtc < EndDateTimeUtc`. |
| Interval model | A booking is represented as a half-open interval: `[start, end)`. |
| Boundary rule | A booking ending exactly when another one begins is allowed. |
| Cancellation | Soft cancellation. The record stays in the database for history but no longer blocks availability. |
| Overlap checks | Only active bookings participate in overlap validation. |
| Resource/user validation | A booking cannot be created for an unknown resource or unknown user. |
| Priority | Correctness and clear design were prioritized over premature optimization. |

---

## Overlap Definition

A booking uses a **half-open interval**:

```text
[start, end)
```

Two active bookings overlap only when this condition is true:

```csharp
existing.StartDateTimeUtc < requested.EndDateTimeUtc &&
requested.StartDateTimeUtc < existing.EndDateTimeUtc
```

### Examples

| Existing booking | New booking | Result |
|---|---|---|
| `09:00–10:00` | `10:00–11:00` | Allowed — they touch but do not overlap. |
| `09:00–10:00` | `09:30–10:30` | Rejected — partial overlap. |
| `09:00–10:00` | `08:30–09:30` | Rejected — partial overlap. |
| `09:00–10:00` | `08:00–11:00` | Rejected — new booking fully contains the existing one. |
| `09:00–10:00` | `09:00–10:00` | Rejected — identical time range. |

This definition is explicit, simple to validate, and correctly supports adjacent bookings.

---

## Concurrency Extension

### Problem

A simple "check then insert" flow has a race condition:

```text
Request A checks for overlap -> no overlapping booking is found
Request B checks for overlap -> no overlapping booking is found
Request A inserts its booking
Request B inserts its booking
```

Both requests can succeed, producing overlapping bookings for the same resource.

### Implemented approach

Booking creation is protected by a resource-level lock through `IResourceBookingLock`.

For a given resource, the flow is:

```text
Acquire lock for the resource
    -> check whether an active overlap exists
    -> insert the booking when no overlap exists
    -> save changes
Release lock
```

The infrastructure implementation uses a SQL Server application lock (`sp_getapplock`) within a database transaction. The lock key is based on the resource identifier, for example:

```text
booking:meeting-room-a
```

This makes the overlap check and insert operation effectively serialized for one resource, including when multiple API instances use the same SQL Server database.

### Trade-offs

- **Benefit:** prevents race conditions for the same resource.
- **Benefit:** bookings for different resources can still be created concurrently.
- **Trade-off:** requests for one highly-contended resource are serialized.
- **Trade-off:** the implementation is SQL Server-specific.
- **Trade-off:** lock acquisition can time out, so a client may need to retry the request.

`RowVersion` alone would not solve this case because two concurrent requests create two different booking rows; the problem is not an update conflict on the same row.

---

## API Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/resources` | Gets the seeded resources. |
| `GET` | `/api/users` | Gets the seeded users. |
| `POST` | `/api/bookings` | Creates a booking. |
| `GET` | `/api/bookings/resource/{resourceId}` | Gets bookings for one resource with filtering and paging. |
| `POST` | `/api/bookings/{bookingId}/cancel` | Soft-cancels a booking. |

### Create booking

```http
POST /api/bookings
Content-Type: application/json
```

```json
{
  "resourceId": "meeting-room-a",
  "userId": "user-001",
  "startDateTime": "2026-07-03T09:00:00Z",
  "endDateTime": "2026-07-03T10:00:00Z"
}
```

### Retrieve bookings

```http
GET /api/bookings/resource/meeting-room-a?fromUtc=2026-07-01T00:00:00Z&toUtc=2026-07-31T23:59:59Z&includeCancelled=true&pageNumber=1&pageSize=20
```

### Cancel booking

```http
POST /api/bookings/{bookingId}/cancel
```

### Expected response codes

| Status | Meaning |
|---|---|
| `201 Created` | Booking was created successfully. |
| `200 OK` | Request succeeded. |
| `204 No Content` | Booking was cancelled successfully. |
| `400 Bad Request` | Input is invalid, such as an invalid date range. |
| `404 Not Found` | A booking, resource, or user was not found. |
| `409 Conflict` | The requested booking overlaps an active booking, or a conflicting booking state was found. |

Swagger is available when the API is running at:

```text
/swagger
```

---

## Running the Project Locally

### Prerequisites

- .NET 10 SDK
- SQL Server LocalDB, SQL Server Express, or SQL Server
- Node.js LTS and npm
- Angular CLI, if it is not installed globally

### 1. Configure the database connection

Update the connection string in:

```text
src/BookingManagement.Api/appsettings.Development.json
```

Example:

```json
{
  "ConnectionStrings": {
    "BookingDb": "Server=(localdb)\\MSSQLLocalDB;Database=BookingManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### 2. Apply database migrations

From the solution root:

```bash
dotnet ef database update --project src/BookingManagement.Infrastructure --startup-project src/BookingManagement.Api
```

Or from Visual Studio Package Manager Console:

```powershell
Update-Database -Project BookingManagement.Infrastructure -StartupProject BookingManagement.Api
```

### 3. Run the API

```bash
dotnet run --project src/BookingManagement.Api
```

Use the URL printed in the terminal or Visual Studio output, then open:

```text
https://localhost:<port>/swagger
```

### 4. Run the frontend

```bash
cd frontend/booking-management-ui
npm install
ng serve
```

Open the URL printed by Angular, normally:

```text
http://localhost:4200
```

Ensure the frontend API base URL matches the API address configured by your local environment.

---

## Testing

Run all backend unit tests from the solution root:

```bash
dotnet test
```

The core tests cover scenarios such as:

- Rejecting an invalid time range where start is equal to or after end.
- Rejecting an overlapping active booking.
- Allowing adjacent bookings.
- Rejecting booking creation when a resource does not exist.
- Rejecting booking creation when a user does not exist.
- Ensuring cancelled bookings do not block a new booking in the same time window.
- Cancelling an active booking and rejecting invalid cancellation operations.

A pass-through implementation of `IResourceBookingLock` is used in unit tests so business behavior can be tested without a real SQL Server lock.

---

## Assumptions and Scope Boundaries

The following items are intentionally not included:

- Authentication, authorization, roles, and permissions.
- CRUD management for resources and users.
- Notifications or email messages.
- Recurring bookings.
- Availability-slot generation.
- Auditing/event sourcing.
- Microservices or message queues.

These are reasonable next steps in a production system, but excluding them keeps the take-home implementation focused on the requested booking behavior and its correctness.

---

## Scale Considerations

The first likely bottlenecks are:

1. The overlap lookup for a frequently booked resource.
2. Lock contention when many users try to book the same resource at once.

The database should have an index aligned with the most common overlap query, starting with fields such as:

```text
ResourceId, Status, StartDateTimeUtc
```

At larger scale, query optimization, archived historical bookings, partitioning strategies, caching for read-heavy resource data, and observability would become relevant.

---

## Distributed-System Evolution

The current design can evolve incrementally:

1. Keep all writes for a resource routed through the booking service that owns that resource.
2. Preserve SQL Server as the synchronization point while all booking writers share the same database.
3. Add idempotency keys to prevent duplicate creates caused by client retries.
4. Use an outbox table to publish booking-created and booking-cancelled integration events reliably.
5. Extract resource/user data into separate services only when ownership, scale, or organizational boundaries require it.
6. Add monitoring for slow overlap queries, lock waits, conflict rates, and API failures.

The design intentionally starts with a reliable single-service architecture rather than prematurely introducing distributed complexity.

---

## Main Trade-off

The main priority was **correctness**.

A booking system must not double-book a resource. The solution therefore accepts a small amount of serialization for bookings of the same resource in exchange for preserving the core invariant: **an active resource cannot have overlapping active bookings**.

The architecture remains lightweight to also preserve simplicity, while the selected concurrency mechanism provides stronger correctness than a basic pre-insert overlap query alone.

---

## Submission Checklist

Before submitting the repository:

- [ ] `dotnet build` succeeds from a clean checkout.
- [ ] `dotnet test` succeeds.
- [ ] Database migrations apply successfully.
- [ ] Swagger starts and all endpoints are reachable.
- [ ] The frontend can create, list, and cancel bookings.
- [ ] Overlapping bookings return `409 Conflict`.
- [ ] Adjacent bookings are allowed.
- [ ] The repository includes this README, API source, frontend source, migrations, and tests.

