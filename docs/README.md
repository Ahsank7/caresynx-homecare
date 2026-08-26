# Project Docs

This folder contains high-level documentation for the CaresynX codebase.

## Documents

- [ARCHITECTURE.md](./ARCHITECTURE.md): system overview, repository layout, runtime flow, and key integration points
- [UI.md](./UI.md): frontend structure, routing, state patterns, shared components, and UI conventions
- [API.md](./API.md): backend startup, layering, dependency injection, response handling, and major modules
- [DATABASE.md](./DATABASE.md): SQL project layout, core tables, stored procedure patterns, and deployment notes
- [MCP.md](./MCP.md): chatbot and MCP integration approach, current scaffold, and recommended rollout plan
- [MCP-FLOW.md](./MCP-FLOW.md): practical explanation of how MCP works in this project from both user and implementation perspectives

## Repository At A Glance

- `Scheduler.Client/eXtream-scheduler`: React 18 frontend using Mantine and React Router
- `Scheduler.API`: .NET 8 Web API using controllers, repositories, and Dapper
- `Scheduler.DB`: SQL Server database project containing tables, functions, stored procedures, and post-deploy seed scripts

## Recommended Reading Order

1. Start with [ARCHITECTURE.md](./ARCHITECTURE.md)
2. Read [UI.md](./UI.md) for frontend work
3. Read [API.md](./API.md) for backend/API work
4. Read [DATABASE.md](./DATABASE.md) for schema, stored procedures, and deployment work
