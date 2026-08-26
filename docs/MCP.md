# MCP Plan

## Goal

Expose your existing Scheduler API as MCP tools so an admin chatbot can perform operational tasks through natural language.

## Recommended Architecture

```text
Admin Chat UI
    ->
LLM / Chat Orchestrator
    ->
Scheduler.MCP
    ->
Scheduler.API
    ->
Scheduler.DB
```

## Why This Fits Your Project

Your codebase already has:

- clean domain modules
- frontend service contracts that reveal API shape
- centralized HTTP and response conventions
- backend controllers and repository layers
- stored procedures for operational workflows

That means MCP does not replace your backend. It simply turns backend capabilities into AI-callable tools.

## Starter Scaffold Added

A starter server now exists at:

- `Scheduler.MCP/`

Key files:

- `Scheduler.MCP/package.json`
- `Scheduler.MCP/.env.example`
- `Scheduler.MCP/src/index.js`
- `Scheduler.MCP/src/config.js`
- `Scheduler.MCP/src/apiClient.js`
- `Scheduler.MCP/src/toolDefinitions.js`
- `Scheduler.MCP/src/toolHandlers.js`
- `Scheduler.MCP/src/httpBridge.js`

## Current Tool Set

Read tools:

- `users.search`
- `users.get_profile`
- `availability.list`
- `schedule.get_client_tasks`
- `schedule.get_service_provider_tasks`
- `planboard.tasks`
- `to_confirm.tasks`

Write tools:

- `users.create`
- `users.update_profile`
- `address.create`
- `contact.create`
- `schedule.create`

The scaffold also includes an optional HTTP bridge so the frontend can call the same handlers directly.

Default local endpoints:

- `GET /health`
- `GET /tools`
- `POST /tools/call`

## Safety Model In The Scaffold

The scaffold already includes two basic protections:

1. Write tools require `MCP_WRITE_ENABLED=true`
2. Write tools require `confirm: true`

This is only the first layer. In production you should also add:

- dedicated chatbot service account
- server-side audit logging
- role-aware tool exposure
- approval prompts before sensitive actions
- restricted write actions for billing, wage, delete, or purge flows

## Suggested Next Tool Additions

### Profile management

- `address.update`
- `contact.update`

### Scheduling operations

- `planboard.assign_service_provider`
- `planboard.update_task_status`
- `planboard.update_task_notes`

### Finance and confirmation

- `billing.preview`
- `wage.preview`
- `to_confirm.calculate_billing_and_wage`

### Read-only admin support

- `notifications.list`
- `login_history.list`
- `task_logs.get`

## Best First Rollout

### Phase 1

Use MCP only for read operations:

- search users
- get profile
- check availability
- list planboard tasks

### Phase 2

Add low-risk writes:

- create user
- create schedule

### Phase 3

Add controlled operational writes:

- assign provider
- update task status
- confirm tasks

## Important Implementation Note

Most frontend services already show the API route names, which makes MCP mapping straightforward. The best ongoing pattern is:

- keep MCP handlers thin
- keep business rules in `Scheduler.API`
- avoid duplicating backend validation in the chatbot layer

## Production Advice

- prefer short-lived tokens or service-to-service auth over manually pasted bearer tokens
- add structured logs for every MCP tool call
- keep destructive actions disabled until approval workflows exist
- consider a dedicated admin chatbot page inside the franchise/admin portal
