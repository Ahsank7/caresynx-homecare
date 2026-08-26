# UI / Frontend

## Stack

- React 18
- React Router 6
- Mantine UI
- Mantine Form
- Mantine Notifications
- Mantine DataTable
- Axios
- Zod
- Moment
- Chart.js / react-chartjs-2

Frontend package details are defined in `Scheduler.Client/eXtream-scheduler/package.json`.

## Main Source Layout

```text
Scheduler.Client/eXtream-scheduler/src
|- App.jsx
|- index.js
|- core/
|- features/
|- shared/
|- styles/
|- enviroment.js
|- enviroment.development.js
|- enviroment.production.js
```

## Folder Responsibilities

### `core`

Shared application infrastructure:

- app shell and layouts
- HTTP services
- auth/session helpers
- context providers
- enums
- common utilities

Important sub-areas usually include:

- `core/components`
- `core/context`
- `core/services`
- `core/utils`

### `features`

Page-oriented feature modules. This is where routed screens live. Current feature coverage includes:

- auth
- organization
- franchise
- planboard
- toConfirm
- user
- billing
- wage
- attendance
- reports
- admin
- landing
- loginHistory

### `shared`

Reusable pieces shared across features:

- profile tabs
- modals
- drawers
- tables
- user profile components
- helper display components

### `styles`

Global or feature-wide CSS used mainly for landing/public pages and supplemental styling.

## Routing

Routes are declared in `Scheduler.Client/eXtream-scheduler/src/App.jsx`.

Main route groups:

- `/organizations/...`
- `/franchises/:franchiseName/dashboard`
- `/franchises/:franchiseName/planboard`
- `/franchises/:franchiseName/toConfirm`
- `/franchises/:franchiseName/profile/...`
- `/franchises/:franchiseName/reports`
- `/login`
- `/attendance`
- `/admin`
- public marketing pages at `/`, `/home`, `/products`, `/contact`

Profile routes use:

- list pages: `profile/clients`, `profile/service-providers`, `profile/staffs`
- detail page: `profile/:userID/:userType`

## Data Access Pattern

The frontend generally follows this flow:

1. Component/page gathers UI input.
2. Component calls a service from `core/services`.
3. Service uses the shared HTTP client.
4. `httpService.js` adds auth headers and unwraps API responses.
5. Component updates state and displays notifications.

### Shared HTTP Behavior

`core/services/httpService.js` handles:

- Axios client configuration
- bearer token injection
- 401 handling and logout redirect
- standardized API response processing through `core/utils/responseHandler.js`

This is an important convention: most service methods already return the unwrapped `data` payload rather than the raw HTTP response.

## UI Patterns In The Codebase

### Layouts

The app uses layout wrappers such as:

- `OrganizationLayout`
- `FranchiseLayout`
- `PublicSiteLayout`

These define navigation, top-level chrome, and nested route rendering.

### Feature Pages

Feature pages tend to:

- load lookup data in `useEffect`
- store filters in local state
- call API services on filter/apply/reset
- use Mantine `LoadingOverlay` during fetches
- render tabular data with `mantine-datatable`

### Shared Components

Common patterns include:

- `AppContainer`: page card and header wrapper
- `AppDrawer`: reusable filter drawer
- `FilterSection`: grouped filter controls within drawers
- `ProfileTabPanel`: standardized profile tab section layout
- `TruncatedTooltipText`: table-cell overflow handling
- `AppConfirmationModal`: confirm destructive actions

### Forms

Forms often use:

- Mantine `useForm`
- `zodResolver`
- inline validation
- field-level `maxLength`
- API response notifications on success/failure

## State Management

The app mostly uses local React state and context rather than a global state library.

Context examples include:

- franchise selection/context
- permissions

This keeps state close to the feature, but it means fetch/reset behavior should be checked carefully when filters are state-driven.

## UI Conventions

Conventions already present in the repo:

- shared service wrappers instead of inline fetch calls
- profile modules split into focused components such as Address, Contact, Availability, Document, Payments
- DataTable-based list pages
- reusable app-shell primitives under `shared/components`
- notifications for save/update/delete feedback

## Frontend Entry Points Worth Knowing

- `src/App.jsx`: route map
- `src/index.js`: React bootstrap
- `src/core/services/httpService.js`: HTTP and auth behavior
- `src/shared/components/index.jsx`: shared component barrel
- `src/shared/components/user/ProfileDetail.jsx`: main profile detail composition

## Suggested Onboarding Path For UI Work

1. Read `App.jsx` to understand route structure.
2. Read `core/services/httpService.js` and a few domain services.
3. Read one feature page such as `features/user/pages/Users.jsx`.
4. Read profile composition in `shared/components/user/ProfileDetail.jsx`.
5. Reuse shared components before creating new primitives.

## Improvement Opportunities

- add a frontend architecture decision record for shared UI patterns
- document naming conventions for `features`, `core`, and `shared`
- add component testing guidance
- add a route map with permissions and intended users
