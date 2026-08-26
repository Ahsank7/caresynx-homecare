# MCP Flow

## Purpose

This document explains:

- what MCP is
- why it fits this project
- how the current CaresynX implementation works
- how admins and developers use it
- what is already done and what can be extended next

This is the practical companion to [MCP.md](./MCP.md).  
`MCP.md` is the rollout/design note.  
`MCP-FLOW.md` explains the working flow in more detail.

## What MCP Is

MCP stands for `Model Context Protocol`.

In simple terms, it is a standard way for an AI model to use tools.

Without MCP, an AI chat assistant only sees text and has to guess or rely on whatever data is pasted into the conversation.

With MCP, the AI can call structured tools such as:

- `users.search`
- `users.get_profile`
- `availability.list`
- `schedule.create`

That means the model is no longer limited to static text. It can ask the system for live data and then use that data to answer the user.

## MCP In Plain Language

Think of MCP as a translator between:

- the AI model
- your application features

The AI says:

`I need to search users.`

MCP translates that into:

`Call the real Users/List API with the right parameters.`

Then MCP gives the result back to the AI so it can answer the user naturally.

So MCP is not your business logic.

Your business logic is still in:

- `Scheduler.API`
- `Scheduler.DB`

MCP is the tool layer that makes those capabilities usable by an AI assistant.

## Why MCP Fits This Project

This project already has:

- a working React admin frontend
- a .NET API with domain-based controllers
- a SQL database project
- clear service flows for users, schedules, availability, billing, and tasks

That means the hard part already exists.

We did not need to rebuild the system for AI.

Instead, we added a thin MCP layer on top of the existing API so the AI can:

- search users
- read profile data
- check availability
- inspect planboard and to-confirm tasks
- create or update records through approved write tools

## Current Architecture

The working architecture is:

```text
Admin User
   ->
React Admin Copilot Page
   ->
MCP HTTP Bridge (Scheduler.MCP)
   ->
OpenAI Chat Orchestration or Direct Tool Execution
   ->
Scheduler.API
   ->
Scheduler.DB
```

There are two ways the system is used:

1. `Tool Runner`
   The admin explicitly selects a tool and runs it with JSON input.
2. `Chat Assistant`
   The admin asks a natural-language question and the LLM decides which read tools to use.

## Repository Locations

### Frontend

- `Scheduler.Client/eXtream-scheduler/src/features/admin/pages/AdminCopilot.jsx`
- `Scheduler.Client/eXtream-scheduler/src/core/services/mcpChatService.js`
- `Scheduler.Client/eXtream-scheduler/src/core/services/localStoreService.js`

### MCP server

- `Scheduler.MCP/src/index.js`
- `Scheduler.MCP/src/httpBridge.js`
- `Scheduler.MCP/src/config.js`
- `Scheduler.MCP/src/apiClient.js`
- `Scheduler.MCP/src/toolDefinitions.js`
- `Scheduler.MCP/src/toolHandlers.js`
- `Scheduler.MCP/src/openaiChat.js`

## User Perspective

### Tool Runner

This is the explicit admin/developer mode.

The user:

1. opens the Copilot page
2. switches to `Tool Runner`
3. selects a tool
4. reviews or edits JSON arguments
5. runs the tool

If the tool is a write operation, the user must approve it before it runs.

This mode feels similar to Swagger or Postman, but it uses the same MCP layer that the AI assistant uses.

This is useful for:

- debugging
- learning the tool contracts
- controlled admin operations
- validating payloads before enabling more AI automation

### Chat Assistant

This is the natural-language mode.

The user can ask:

- `How many Fareeha users are registered, what are their ages, and give me the email of the youngest`
- `Show me planboard tasks for tomorrow`
- `Check the availability of provider X for next week`

The user does not need to know API routes or JSON payloads.

The assistant:

1. interprets the request
2. picks the right read tools
3. gets live data
4. summarizes the result in normal language

At the moment, chat mode is intentionally read-only.

That keeps the first version safer and easier to trust.

## Implementation Perspective

## 1. Frontend Context Collection

The frontend does not ask the admin to manually enter token or IDs.

It uses [localStoreService.js](../Scheduler.Client/eXtream-scheduler/src/core/services/localStoreService.js) to read:

- token
- organization ID
- franchise ID
- user ID

These are forwarded to the MCP bridge in request headers by [mcpChatService.js](../Scheduler.Client/eXtream-scheduler/src/core/services/mcpChatService.js).

That means the MCP calls run in the context of the currently logged-in admin.

## 2. MCP HTTP Bridge

The browser cannot speak stdio MCP directly, so we added a lightweight HTTP bridge.

Bridge endpoints:

- `GET /health`
- `GET /config-check`
- `GET /tools`
- `POST /tools/call`
- `POST /chat`

This bridge lives in [httpBridge.js](../Scheduler.MCP/src/httpBridge.js).

It does three important jobs:

- handles browser requests
- supports CORS for the frontend
- passes request context to MCP handlers

## 3. Direct Tool Execution

When Tool Runner is used:

1. frontend calls `POST /tools/call`
2. bridge looks up the requested tool handler
3. handler prepares the payload
4. handler calls the real API through [apiClient.js](../Scheduler.MCP/src/apiClient.js)
5. result returns to the frontend

This is direct and deterministic.

No LLM is involved in manual tool execution.

## 4. LLM Chat Execution

When Chat Assistant is used:

1. frontend calls `POST /chat`
2. bridge calls the internal chat handler
3. [openaiChat.js](../Scheduler.MCP/src/openaiChat.js) sends the prompt to OpenAI
4. the model sees the available read-only tools
5. if it needs live data, it calls one or more tools
6. MCP executes those tools
7. tool results are returned to the model
8. the model produces a final natural-language answer

This is the main value of MCP.

Instead of hardcoding every possible admin query, we let the model:

- understand the user’s language
- choose the correct tool
- summarize the result

## 5. API Execution

MCP does not replace your backend.

Every business operation still goes to the real `Scheduler.API`.

Examples:

- `users.search` -> `Users/List`
- `users.get_profile` -> `Users/Details`
- `availability.list` -> `Availability/List`
- `planboard.tasks` -> `PlanBoard/ServicesTask`
- `to_confirm.tasks` -> `ToConfirm/ServicesTask`
- `schedule.create` -> `Scheduler/CreateAppointment`

That means all real rules, validation, persistence, and database behavior remain in the API and DB where they belong.

## Tool Model

The tool contracts are declared in [toolDefinitions.js](../Scheduler.MCP/src/toolDefinitions.js).

The actual logic is in [toolHandlers.js](../Scheduler.MCP/src/toolHandlers.js).

Current read tools:

- `users.search`
- `users.get_profile`
- `availability.list`
- `schedule.get_client_tasks`
- `schedule.get_service_provider_tasks`
- `planboard.tasks`
- `to_confirm.tasks`

Current write tools:

- `users.create`
- `users.update_profile`
- `address.create`
- `contact.create`
- `schedule.create`

## Safety Model

Write actions are protected in two ways:

1. `MCP_WRITE_ENABLED=true` must be enabled
2. `confirm: true` must be present before the handler allows a write

In the UI, that confirmation is surfaced through the approval modal on the Copilot page.

This gives a controlled path for create/update actions while still allowing read-only AI assistance.

## Why Chat Mode Is Read-Only Right Now

Natural-language write automation is powerful, but it also introduces higher risk.

For example:

- wrong user updated
- wrong schedule created
- billing or wage side effects
- unintended provider assignments

The current design intentionally keeps:

- chat = read tools only
- writes = manual tool runner with approval

This is the safest first production-style shape.

## OpenAI Usage

Chat mode uses the OpenAI Responses API.

The MCP server sends:

- system instructions
- conversation history
- tool definitions

The model can then decide whether to answer directly or call tools first.

This is configured through:

- `OPENAI_API_KEY`
- `MCP_OPENAI_MODEL`
- `MCP_OPENAI_REASONING_EFFORT`

in `Scheduler.MCP/.env`.

## Important Local Development Notes

For local development, `Scheduler.MCP` is a separate Node process and must be started independently.

Typical local setup:

1. start React frontend
2. start .NET API
3. start `Scheduler.MCP`

The MCP project reads [Scheduler.MCP/.env](../Scheduler.MCP/.env) automatically at startup.

Useful checks:

- `http://localhost:8787/health`
- `http://localhost:8787/config-check`

The config-check endpoint is especially useful for debugging:

- whether `.env` was found
- whether the OpenAI key is loaded
- which model is active
- whether bridge settings are correct

## Typical Request Examples

### Example 1: Manual tool execution

Admin action:

`Search client users named Fareeha`

Tool Runner payload:

```json
{
  "franchiseId": "<current-franchise-id>",
  "userType": 1,
  "firstName": "Fareeha",
  "pageNumber": 1,
  "pageSize": 10
}
```

### Example 2: Natural-language query

Admin question:

`How many Fareeha users are registered, what are their ages, and give me the email of the youngest`

Internal flow:

1. model calls `users.search`
2. MCP gets matching records
3. model calculates count, ages, youngest user
4. model returns the final answer

## What Has Been Solved In This Implementation

- frontend session context is reused automatically
- browser can communicate with MCP through HTTP
- CORS support was added for local frontend usage
- live API calls work through MCP
- write tools use approval flow
- natural-language chat mode works through OpenAI plus MCP read tools
- config diagnostics were added for easier debugging

## Current Constraints

- chat mode does not yet perform writes
- tool coverage is still focused on users, availability, schedule, planboard, and to-confirm
- there is not yet a full audit log for MCP actions
- there is not yet a role-aware tool exposure model

## Recommended Next Enhancements

1. Allow chat to propose write actions and route them into the existing approval modal
2. Add audit logging for MCP actions in API or DB
3. Add more tools for billing, wage, notifications, complaints, and login history
4. Add role-based tool visibility
5. Add richer admin summaries and reporting flows

## Summary

MCP in this project is a bridge between AI and your existing Scheduler platform.

It does not replace your API.
It does not replace your database.
It does not invent business rules.

It gives the AI a safe, structured way to use the system you already built.

That is why it is a good fit here:

- your API remains the source of truth
- MCP becomes the tool layer
- the admin gets both:
  - explicit tool control
  - natural-language AI assistance
