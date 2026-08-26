# Scheduler MCP Server

This folder contains a starter MCP server for the CaresynX Scheduler project.

The idea is to expose safe, structured tools on top of your existing API so an admin chatbot can:

- search users
- get a user profile
- check availability
- create a user
- update a user profile
- create an address
- create a contact
- create a schedule
- view planboard tasks
- view to-confirm tasks
- answer natural-language admin questions through an LLM-backed chat mode

## What This Is

This is a thin adapter layer:

- chatbot or AI client talks to MCP
- MCP tools call your existing `Scheduler.API`
- `Scheduler.API` continues to own business logic
- `Scheduler.DB` remains the source of truth

## Current Tool Coverage

### Read tools

- `users.search`
- `users.get_profile`
- `availability.list`
- `schedule.get_client_tasks`
- `schedule.get_service_provider_tasks`
- `planboard.tasks`
- `to_confirm.tasks`

### Write tools

- `users.create`
- `users.update_profile`
- `address.create`
- `contact.create`
- `schedule.create`

Write tools are guarded by:

- `MCP_WRITE_ENABLED=true`
- explicit `confirm: true` in tool arguments

The hybrid Copilot now has two modes:

- `Chat Assistant`: natural-language, read-only LLM mode that can call MCP read tools
- `Tool Runner`: explicit tool execution for admin/developer workflows, including write tools with approval

## Environment Variables

For local development, this project now reads `Scheduler.MCP/.env` automatically if it exists.
The repo includes a starter `.env` with the local API URL and bridge port already set.

You can still copy `.env.example` and expand it if you want explicit defaults for org, franchise, or acting user.

- `MCP_API_BASE_URL`: scheduler API base URL without trailing slash requirement, for example `http://localhost:5000/api`
- `MCP_API_TOKEN`: optional fallback bearer token for an admin-capable API user. The browser Copilot now forwards the logged-in token automatically.
- `OPENAI_API_KEY`: required for natural-language chat mode
- `MCP_OPENAI_MODEL`: optional OpenAI model name for chat mode. Defaults to `gpt-5.4-mini`.
- `MCP_OPENAI_REASONING_EFFORT`: optional reasoning effort for chat mode. Defaults to `low`.
- `MCP_DEFAULT_ORGANIZATION_ID`: optional default organization for admin operations
- `MCP_DEFAULT_FRANCHISE_ID`: optional default franchise for user/schedule/task operations
- `MCP_ACTING_USER_ID`: optional default admin/service user GUID for schedule creation
- `MCP_WRITE_ENABLED`: set to `true` to allow create/update actions
- `MCP_HTTP_PORT`: optional HTTP bridge port for browser/admin UI access
- `MCP_ALLOW_INSECURE_LOCAL_HTTPS`: local-dev helper for self-signed localhost certificates. Keep this `true` for local .NET HTTPS, and turn it off outside dev.

## Install

```bash
cd Scheduler.MCP
npm install
```

## Run

```bash
npm start
```

The server uses stdio transport, which is the normal pattern for MCP clients.

If `MCP_HTTP_PORT` is set, the same process also starts a lightweight HTTP bridge for browser/admin UI use.

Default local bridge endpoints:

- `GET /health`
- `GET /config-check`
- `GET /tools`
- `POST /tools/call`
- `POST /chat`

## Suggested First Admin Chat Use Cases

1. Search for a user by name, email, or phone
2. Check a service provider's availability
3. List planboard tasks for a date range
4. Create a new client or staff profile
5. Update profile/contact/address details with approval
6. Create a schedule for a client and provider

## Recommended Safety Rules

- keep destructive actions out of the first version
- require `confirm: true` on all write tools
- use a dedicated admin service account token
- add chatbot-side confirmation before writes
- log all tool calls for auditability

## How It Maps To Your Existing API

The scaffold currently maps tools to endpoints already present in the repo:

- `Users/List`
- `Users/Details`
- `Users/SaveUpdate`
- `Availability/List`
- `Scheduler/CreateAppointment`
- `Scheduler/GetClientTasks`
- `Scheduler/GetServiceProviderTasks`
- `PlanBoard/ServicesTask`
- `ToConfirm/ServicesTask`

## Good Next Steps

1. Add authentication/login token refresh support
2. Add more tools for address, contact, and profile updates
3. Add role-aware tool exposure
4. Add audit logging table or API endpoint for chatbot actions
5. Add approval workflows for billing/wage or delete actions
