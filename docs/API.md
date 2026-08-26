# API / Backend

## Stack

- .NET 8 ASP.NET Core Web API
- Dapper
- SQL Server
- Swagger / OpenAPI
- JWT authentication
- FluentValidation

## Main Backend Structure

```text
Scheduler.API
|- Program.cs
|- Controllers/
|- Services/
|- Models/
|- Extensions/
|- Application/Middlewares/
|- Common/
```

## Startup Flow

`Program.cs` is the application entry point.

The startup sequence is:

1. Create web application builder
2. Load AWS Secrets Manager values in production
3. Register app services, Swagger, and CORS
4. Register authentication
5. Build the app
6. Apply middleware:
   - HTTPS redirection
   - CORS
   - global exception handling
   - authentication
   - authorization
7. Map controllers
8. enable Swagger UI
9. serve static files for uploaded content

## Dependency Injection

Dependency registration is centralized in `Extensions/ServiceCollectionExtensions.cs`.

This file wires up:

- Dapper repository abstraction
- domain repositories for each business area
- file storage factory and service
- Stripe/payment services
- email services
- notification and preference services
- validators

## Layering Pattern

The backend generally uses this structure:

### Controllers

- receive HTTP requests
- validate basic request presence
- delegate to service/repository interfaces
- use `ExecuteAsync` patterns from `BaseController`
- return standardized response envelopes

Examples:

- `UsersController`
- `PlanBoardController`
- `BillingController`
- `PaymentController`
- `ToConfirmController`

### Services / Repositories

- encapsulate Dapper and business logic
- one main service namespace per domain
- often named `XRepository` implementing interface `IX`

Examples:

- `Services/User/UserRepository.cs`
- `Services/PlanBoard/PlanBoardRepository.cs`
- `Services/Billing/BillingRepository.cs`
- `Services/Wage/WageRepository.cs`

### Models

Models are grouped by business domain and typically include:

- request models
- response models
- DTO/view models
- save/update payloads

Examples:

- `Models/User`
- `Models/Billing`
- `Models/ToConfirm`
- `Models/Complaint`

## Response Model

The API uses a common response wrapper in `Common/Response.cs`.

Typical shape:

- `status`
- `message`
- `data`
- `isSuccess`
- `errors`
- `traceId`

This is important because the frontend `handleApiResponse` helper unwraps the `data` property and throws based on the success metadata.

## Authentication And Security

Authentication is configured through extension methods and middleware.

Important pieces:

- JWT bearer auth
- custom middleware under `Application/Middlewares`
- logout/login history support
- role/permission endpoints and database support

Relevant files:

- `Application/Middlewares/CustomAuthenticationMiddleware.cs`
- `Application/Middlewares/CustomJwtBearerEvents.cs`
- `Extensions/AuthenticationExtensions.cs`

## CORS

CORS is configured in `AddAppCors`.

Allowed origins include:

- local development
- production domains
- cloud-hosted frontend URLs

If a new environment is added, this list usually needs updating.

## Static File Serving

The API serves uploaded files from configured local storage paths.

By default, it creates and exposes:

- `/ProfileImages`
- `/OrganizationLogos`
- `/UserDocument`

The storage path can come from configuration and may be local or cloud-backed through the file storage abstraction.

## Major Domain Modules

The service layer currently includes modules for:

- user
- client
- staff
- service provider
- address
- contact
- availability
- leave
- document
- organization and franchise
- scheduler and planboard
- billing and wage
- payment and transactions
- role and permissions
- login history
- notifications
- preferences
- complaints

## Data Access

Dapper is used through `IDapperRepository` and `DapperRepository`.

Most domain repositories:

- build `DynamicParameters`
- call stored procedures
- map results to domain models
- return typed data to controllers

This makes the DB project part of the effective backend contract.

## Files Worth Reading First

- `Program.cs`
- `Extensions/ServiceCollectionExtensions.cs`
- `Controllers/BaseController.cs`
- `Common/Response.cs`
- `Services/DapperRepository.cs`
- one domain pair such as `UsersController.cs` and `Services/User/UserRepository.cs`

## Backend Development Notes

- keep controllers thin
- preserve the standard response envelope
- prefer existing service/repository patterns before introducing new ones
- if a change needs SQL behavior, update both API and DB projects together
- check file storage, notification, or login history side effects when modifying auth/profile flows

## Improvement Opportunities

- add endpoint inventory by controller
- add auth/permission matrix documentation
- document file storage provider selection rules
- add background job documentation for billing, wage, and delayed task processing
