# Ecommerce API

Backend API for a simple e-commerce order management system, developed as a practical Senior .NET technical assessment.

The solution focuses on clean architecture, explicit business rules, testability, maintainability, and clear separation of concerns.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Controllers
- Entity Framework Core
- SQLite
- MediatR
- FluentValidation
- JWT Bearer Authentication
- Serilog
- xUnit
- NSubstitute
- WebApplicationFactory
- Docker / Docker Compose

## Architecture

The solution follows Clean Architecture and is divided into four main projects:

```text
src/
├── Ecommerce.Domain
├── Ecommerce.Application
├── Ecommerce.Infrastructure
└── Ecommerce.Api

tests/
├── Ecommerce.Domain.Tests
├── Ecommerce.Application.Tests
└── Ecommerce.Api.IntegrationTests
```

### Ecommerce.Domain

Contains the core business model and business rules.

The `Order` entity acts as the aggregate root and controls its `OrderItem` collection.

Examples of domain invariants:

- An order must contain at least one item.
- Quantity must be greater than zero.
- Unit price must be greater than zero.
- Only `Pending` orders can be cancelled.
- `TotalAmount` is calculated by the Domain from the order items.

### Ecommerce.Application

Contains the application use cases implemented using CQRS and MediatR.

Commands change application state:

- `CreateOrderCommand`
- `CancelOrderCommand`

Queries retrieve data:

- `GetOrderByIdQuery`
- `GetOrdersQuery`

MediatR pipeline behaviors provide cross-cutting concerns such as:

- FluentValidation
- Structured logging
- Command/query execution time

### Ecommerce.Infrastructure

Contains infrastructure concerns such as:

- Entity Framework Core
- SQLite
- Entity mappings
- Repository implementations
- Database migrations

The Application layer depends on repository abstractions, while Infrastructure provides their implementations.

### Ecommerce.Api

Contains the HTTP interface of the application.

Controllers are intentionally thin and delegate use cases to MediatR.

The API is also responsible for:

- JWT authentication
- HTTP status code mapping
- Global exception handling
- Problem Details responses
- HTTP request logging

## Why Controllers?

Controllers were chosen instead of Minimal APIs because they provide a clear HTTP boundary for this assessment while keeping routing, authorization, and HTTP concerns explicit.

Business logic is not implemented in controllers. Controllers only receive HTTP requests, delegate use cases to MediatR, and translate application results into HTTP responses.

## Authentication

The Orders endpoints require JWT Bearer authentication.

For this practical assessment, authentication uses the fixed credentials required by the specification:

```text
Email: dev@martech.com
Password: Senha@123
```

### Login

```http
POST /auth/login
```

Request:

```json
{
  "email": "dev@martech.com",
  "password": "Senha@123"
}
```

Response:

```json
{
  "accessToken": "<jwt-token>"
}
```

Use the returned token in subsequent requests:

```text
Authorization: Bearer <jwt-token>
```

> For this practical assessment, the JWT signing key is stored in application configuration. In a production environment, secrets should be provided through environment variables or a secret management service.

## API Endpoints

All `/api/orders` endpoints require JWT authentication.

### Create Order

```http
POST /api/orders
```

Example request:

```json
{
  "customerId": "11111111-1111-1111-1111-111111111111",
  "items": [
    {
      "productName": "Keyboard",
      "quantity": 2,
      "unitPrice": 100
    }
  ]
}
```

Successful response:

```text
201 Created
```

Example response:

```json
{
  "id": "7c115c94-80b9-4d32-a7d2-84b29d511c6f"
}
```

### List Orders

```http
GET /api/orders?page=1&pageSize=10
```

Pagination parameters are validated.

- `page` must be greater than zero.
- `pageSize` accepts values from 1 to 100.

Example response:

```json
{
  "page": 1,
  "pageSize": 10,
  "totalCount": 1,
  "items": [
    {
      "id": "7c115c94-80b9-4d32-a7d2-84b29d511c6f",
      "customerId": "11111111-1111-1111-1111-111111111111",
      "status": "Pending",
      "createdAt": "2026-08-29T23:38:28.3396915",
      "totalAmount": 200
    }
  ]
}
```

### Get Order By Id

```http
GET /api/orders/{id}
```

Possible responses:

```text
200 OK          Order found
404 Not Found   Order does not exist
```

Example response:

```json
{
  "id": "7c115c94-80b9-4d32-a7d2-84b29d511c6f",
  "customerId": "11111111-1111-1111-1111-111111111111",
  "status": "Pending",
  "createdAt": "2026-08-29T23:38:28.3396915",
  "totalAmount": 200,
  "items": [
    {
      "id": "12396ba1-2f36-4a00-8fcf-bb249d82ee1a",
      "productName": "Keyboard",
      "quantity": 2,
      "unitPrice": 100
    }
  ]
}
```

### Cancel Order

```http
PATCH /api/orders/{id}/cancel
```

Only orders with `Pending` status can be cancelled.

Possible responses:

```text
204 No Content    Order cancelled
404 Not Found     Order does not exist
409 Conflict      Order cannot be cancelled
```

Attempting to cancel an order that is not `Pending` results in an HTTP `409 Conflict`.

## Validation and Error Handling

Application input validation is implemented with FluentValidation and executed through a MediatR pipeline behavior before the corresponding handler.

Domain invariants are also enforced inside the Domain model.

These mechanisms serve different purposes:

- FluentValidation validates application input.
- Domain validation protects business invariants regardless of the application entry point.

Errors are translated into consistent ASP.NET Core Problem Details responses through global exception handling.

Example validation response:

```json
{
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more validation errors occurred.",
  "errors": {
    "Page": [
      "'Page' must be greater than '0'."
    ]
  }
}
```

A domain state conflict is translated into HTTP `409 Conflict`.

Example:

```json
{
  "title": "Domain conflict",
  "status": 409,
  "detail": "Only pending orders can be cancelled.",
  "instance": "/api/orders/{id}/cancel"
}
```

## Database

The application uses SQLite through Entity Framework Core.

The database schema is versioned using EF Core migrations.

Pending migrations are automatically applied when the application starts:

```csharp
await dbContext.Database.MigrateAsync();
```

This keeps local and Docker startup simple for the practical assessment.

`TotalAmount` is intentionally not persisted because it is derived from the order items by the Domain model.

Read-only queries use `AsNoTracking()`.

Order items are loaded when required because the Domain calculates `TotalAmount` from the item collection.

## Running Locally

### Requirements

- .NET 10 SDK

Clone the repository and restore dependencies:

```bash
dotnet restore
```

Build the solution:

```bash
dotnet build
```

Run the API:

```bash
dotnet run --project src/Ecommerce.Api
```

The development URL can be found in:

```text
src/Ecommerce.Api/Properties/launchSettings.json
```

When running with the current development profile, the API is available at:

```text
http://localhost:5196
```

The SQLite database is created automatically under:

```text
src/Ecommerce.Api/data/ecommerce.db
```

EF Core automatically applies pending migrations during application startup.

## Running the Tests

Run all automated tests:

```bash
dotnet test
```

The test suite contains:

- Domain unit tests
- Application handler unit tests
- API integration tests

### Domain Tests

Domain tests verify business rules independently from Application and Infrastructure.

Examples include:

- Valid order creation
- Order total calculation
- Invalid quantities
- Invalid unit prices
- Required items
- Cancellation rules

### Application Tests

Application handler tests verify use-case orchestration.

Repository abstractions are mocked using NSubstitute so these tests remain isolated from Infrastructure.

### Integration Tests

API integration tests use `WebApplicationFactory` to exercise the real ASP.NET Core pipeline.

An isolated in-memory SQLite database is used by integration tests, preventing them from modifying the developer's local database.

This provides a clear separation between:

```text
Unit Tests
    └── Domain and Application behavior in isolation

Integration Tests
    └── ASP.NET Core HTTP pipeline and application integration
```

## Running with Docker

### Requirements

- Docker
- Docker Compose

Build the Docker image:

```bash
docker compose build
```

Start the application:

```bash
docker compose up
```

The containerized API is exposed at:

```text
http://localhost:8080
```

Example login:

```text
POST http://localhost:8080/auth/login
```

The Docker image uses a multi-stage build.

The .NET SDK image is used only during restore, compilation, and publishing. The final image contains only the ASP.NET Core runtime and the published application.

This keeps the runtime image smaller and avoids shipping the full SDK.

### SQLite Persistence

SQLite data is stored inside the container at:

```text
/app/data/ecommerce.db
```

A named Docker volume is mounted at:

```text
/app/data
```

This allows database data to survive container recreation.

The persistence behavior can be verified with:

```bash
docker compose down
docker compose up
```

Orders previously created remain available because the named volume is preserved.

To stop and remove the container:

```bash
docker compose down
```

To also remove the persisted database volume:

```bash
docker compose down -v
```

> `docker compose down -v` removes the persisted SQLite data.

## Docker Build Context

Local build artifacts and runtime database files are excluded through `.dockerignore`.

Examples:

```text
**/bin/
**/obj/
**/*.db
**/*.db-shm
**/*.db-wal
```

This is particularly important when building a Linux container from a Windows development environment.

The container performs its own package restore and compilation instead of reusing environment-specific build artifacts generated on the host.

## Logging and Observability

Serilog provides structured HTTP request logging.

HTTP request logging records the complete request execution time.

A custom MediatR `LoggingBehavior` records execution time for individual commands and queries.

This provides observability at two different levels:

```text
HTTP Request
    ↓
Serilog Request Logging
    ↓
ASP.NET Core Pipeline
    ↓
MediatR
    ↓
LoggingBehavior
    ↓
ValidationBehavior
    ↓
Command / Query Handler
```

For example, HTTP logging measures the complete request lifecycle, while the MediatR behavior measures application use-case execution.

Structured logging uses named properties instead of string concatenation, allowing log platforms to query individual properties such as request name and execution time.

The application intentionally avoids logging complete command payloads to reduce the risk of exposing sensitive information.

## CQRS and MediatR

The Application layer uses CQRS to explicitly separate operations that modify state from operations that retrieve data.

Commands:

```text
CreateOrderCommand
CancelOrderCommand
```

Queries:

```text
GetOrderByIdQuery
GetOrdersQuery
```

MediatR dispatches these requests to their corresponding handlers.

The HTTP layer therefore does not need to know the implementation details of each use case.

Example flow:

```text
HTTP Request
    ↓
Controller
    ↓
MediatR
    ↓
Pipeline Behaviors
    ↓
Handler
    ↓
Domain
    ↓
Repository Abstraction
    ↓
EF Core / SQLite
```

## Domain Model

`Order` is the aggregate root.

It controls the creation and modification of its `OrderItem` collection.

Consumers can read the items, but modifications are performed through the aggregate.

Entities use private setters to prevent uncontrolled external state changes.

Example concept:

```text
External code
     ↓
Order domain methods
     ↓
Business invariants
     ↓
Internal state changes
```

An `Order` is created in a valid state and requires at least one item.

`OrderItem` validates its own required values such as:

- Product name
- Quantity
- Unit price
- Order identifier

The cancellation state transition is also protected by the Domain:

```text
Pending → Cancelled       Allowed

Confirmed → Cancelled     Rejected
Cancelled → Cancelled     Rejected
```

## Dependency Direction

Dependencies point toward the core of the application.

```text
Ecommerce.Api
      ↓
Ecommerce.Application
      ↓
Ecommerce.Domain

Ecommerce.Infrastructure
      ↓
Ecommerce.Application
      ↓
Ecommerce.Domain
```

The Application layer defines persistence abstractions such as `IOrderRepository`.

Infrastructure implements those abstractions using EF Core.

This keeps application use cases independent from the specific persistence technology.

## Important Design Decisions

Some important decisions made in the solution:

- `Order` is the aggregate root and controls its items.
- Entities use private setters to prevent uncontrolled state changes.
- Orders are created in a valid state.
- Business invariants remain in the Domain.
- CQRS explicitly separates write and read use cases.
- MediatR keeps controllers decoupled from handlers.
- Controllers remain thin and contain no business logic.
- Repository abstractions belong to Application.
- EF Core implementations belong to Infrastructure.
- Read-only EF Core queries use `AsNoTracking()`.
- Pagination uses `Skip()` and `Take()`.
- Pagination returns the total number of records.
- `TotalAmount` is calculated by the Domain rather than persisted.
- FluentValidation runs through a MediatR pipeline behavior.
- Domain rules provide an additional invariant protection layer.
- HTTP errors use Problem Details.
- Invalid domain state transitions are mapped to HTTP `409 Conflict`.
- SQLite migrations are automatically applied at startup.
- Integration tests use an isolated SQLite database.
- Serilog provides structured HTTP request logging.
- MediatR logging records command/query execution time.
- Docker uses a multi-stage build.
- SQLite is persisted through a named Docker volume.
- Local `bin`, `obj`, and database files are excluded from the Docker build context.

## Production Considerations

This project intentionally keeps some concerns simple because it is a practical technical assessment.

For a production system, possible improvements include:

- External identity provider instead of fixed in-memory credentials.
- Secret manager or environment variables for JWT signing keys.
- Production-grade relational database such as PostgreSQL or SQL Server.
- Database migration strategy separated from application startup for multi-instance deployments.
- Distributed tracing and metrics using OpenTelemetry.
- Health checks.
- CI/CD pipeline.
- Additional integration and end-to-end tests.
- Concurrency handling where required.
- Idempotency for operations that require retry safety.
- More advanced authentication and authorization policies.
- Centralized logging and monitoring.

These concerns were intentionally not over-engineered for the scope of the assessment.

## Project Goals

The main goal of this implementation is not feature count.

The solution prioritizes:

- Clear architecture
- Explicit business rules
- Testability
- Maintainability
- Dependency inversion
- Consistent error handling
- Observability
- Reproducible execution
- Simple local and Docker setup

The architecture is intentionally explicit so that business rules, application use cases, infrastructure concerns, and HTTP concerns can evolve independently.

## Author

Daniel Bringel Gusmão