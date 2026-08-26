# Architecture

## Overview

CaresynX is a full-stack scheduling and operations platform for home care workflows. The repository is split into three primary applications:

- `Scheduler.Client/eXtream-scheduler`: browser-based frontend
- `Scheduler.API`: backend HTTP API
- `Scheduler.DB`: SQL Server database project

At runtime, the frontend calls the API, and the API reads and writes data through Dapper-backed repository services and SQL stored procedures in the database.

## Solution Structure

```text
caresynx/
|- Scheduler.Client/eXtream-scheduler
|- Scheduler.API
|- Scheduler.DB
|- README.md
|- Scheduler.sln
```

## High-Level Request Flow

1. A user opens the React application.
2. React Router resolves the current page and renders a feature screen.
3. UI components call service modules under `src/core/services`.
4. Services send HTTP requests through the shared Axios wrapper in `httpService.js`.
5. The .NET API receives the request in a controller.
6. The controller delegates work to a domain service or repository registered through DI.
7. Repository implementations use Dapper and stored procedures to access SQL Server.
8. The API wraps the result in a standard response envelope.
9. The frontend response handler unwraps the payload and updates UI state.

## Main Runtime Boundaries

### Frontend

- Responsible for routing, interaction, validation, notifications, and page composition
- Uses feature folders and shared components
- Handles auth token forwarding through the shared HTTP client

### API

- Responsible for authentication, business orchestration, file access, payment integration, and database access
- Uses controller + repository/service patterns
- Uses a consistent response contract through `Common/Response.cs`

### Database

- Holds core business data for users, franchises, schedules, tasks, billing, wage, lookup values, preferences, complaints, and notifications
- Encapsulates most read/write operations in stored procedures
- Uses a SQL project and post-deployment seeding script

## Frontend Architecture Summary

The frontend is organized around:

- `core`: shared app services, layout, auth, context, enums, and utilities
- `features`: page-level feature areas such as planboard, reports, billing, franchise, organization, and user lists
- `shared`: reusable components, profile tabs, tables, modals, and utilities reused across features
- `styles`: CSS assets for the public/landing experience

Routes are defined in `Scheduler.Client/eXtream-scheduler/src/App.jsx`.

Key routed areas include:

- organization management
- franchise dashboard
- planboard
- to confirm
- profile lists and profile detail
- billing, wage, transactions, reports
- public marketing pages

## API Architecture Summary

The API uses a straightforward layered pattern:

- Controllers: HTTP endpoints and input validation guardrails
- Services/Repositories: business and database interaction logic
- Models: request, response, and data transfer models grouped by domain
- Common/Extensions/Middleware: app startup, exception handling, auth setup, and shared helpers

Service registrations live in `Scheduler.API/Extensions/ServiceCollectionExtensions.cs`.

Important cross-cutting pieces:

- JWT authentication
- global exception handling middleware
- CORS policy
- Swagger/OpenAPI
- file storage abstraction
- Stripe/payment services
- login history tracking

## Database Architecture Summary

The DB project is organized by a mix of schema and business domain:

- `dbo/Tables`: primary table definitions
- `dbo/Stored Procedures`: shared/core procedures
- `dbo/Functions`: scalar and helper functions
- domain folders such as `User`, `CLIENT`, `Staff`, `ServiceProvider`, `Contact`, `Organization`, `Franchise`, `Lookup`, `payment`
- `Script.SchedulerPostDeployment.sql`: seed and post-deploy data setup
- `Scripts/`: operational or one-off helper scripts

The project builds a DACPAC and is intended to be deployed as a SQL Server database project.

## Core Business Domains

This repository centers around a few recurring domains:

- organizations and franchises
- users, clients, service providers, and staff
- services and schedules
- tasks, attendance, and confirmation
- billing and wage generation
- payments and transactions
- lookups and reference data
- notifications, complaints, and preferences

## Integration Points

### Authentication

- Frontend stores and forwards JWTs
- API validates bearer tokens
- login history is tracked in the backend/database

### File Storage

- The API exposes static file paths for profile images, organization logos, and user documents
- File storage is abstracted behind `IFileStorageService`
- Implementations include local storage and cloud-backed variants

### Payments

- Payment and connected account flows exist in the API and DB
- Billing and wage data originate from database procedures and are processed through payment services

## Operational Notes

- Production API configuration can be sourced from AWS Secrets Manager
- Frontend deployment is documented in the root `README.md`
- The API serves uploaded files from local storage directories if configured for local file storage

## Suggested Future Doc Expansion

- sequence diagrams for billing and wage generation
- ERD for key tables
- permission and role model documentation
- deployment runbooks for local, staging, and production environments
