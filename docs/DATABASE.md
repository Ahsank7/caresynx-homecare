# Database

## Overview

`Scheduler.DB` is a SQL Server database project. It contains schema objects, stored procedures, helper functions, operational scripts, and post-deployment seed logic.

The project builds as a DACPAC and is intended to be deployed through SSDT/database project tooling.

## Main Structure

```text
Scheduler.DB
|- dbo/
|  |- Tables/
|  |- Stored Procedures/
|  |- Functions/
|- User/Stored Procedures/
|- CLIENT/Stored Procedures/
|- Staff/Stored Procedures/
|- ServiceProvider/Stored Procedures/
|- Contact/Stored Procedures/
|- Organization/Stored Procedures/
|- Franchise/Stored Procedures/
|- Lookup/Stored Procedures/
|- payment/Stored Procedures/
|- Scripts/
|- Script.SchedulerPostDeployment.sql
|- Scheduler.DB.sqlproj
```

## Object Organization

The DB project mixes schema-based and domain-based organization:

- `dbo/Tables`: core table definitions
- `dbo/Stored Procedures`: shared procedures used across multiple domains
- domain folders: domain-specific procedures grouped by business area
- `Scripts`: operational and maintenance scripts that are not always part of the build

## Core Table Groups

Representative table families include:

### Users And Profile Data

- `tblUser`
- `tblClient`
- `tblStaff`
- `tblServiceProvider`
- `tblUserAddress`
- `tblUserContact`
- `tblUserAvailability`
- `tblUserLeave`
- `tblUserExpense`
- `tblUserRole`
- `tbUserFranchise`

### Scheduling And Tasking

- `tblScheduler`
- `tblServicesTask`
- `tblServices`
- `tblServicesType`
- `tblTaskLog`

### Billing, Wage, And Payments

- `tblBillingInvoice`
- `tblBillingInvoiceDetail`
- `tblServiceProviderWage`
- `tblServiceProviderWageDetail`
- `tblTransaction`
- `tblBankAccount`
- `tblCardInfo`

### Organization And Franchise

- `tblOrganization`
- `tblFranchise`
- `tblFranchiseSetting`
- `tblOrganizationPackage`
- `tblPackage`
- `tblPackageInvoice`
- `tblOrganizationTimeBasedRates`

### Reference And Control Data

- `tblLookups`
- `tblLookupItems`
- `tblMenu`
- `tblRole`
- `tblRolePermission`
- `tblNextUserStatuses`

### Supporting Features

- `tbldocument`
- `tblLoginHistory`
- `tblNotification`
- `tblNotificationRead`
- `tblComplaint`
- `tblClientPreferences`
- `tblServiceProviderAttributes`

## Stored Procedure Pattern

The project leans heavily on stored procedures for both reads and writes.

Common procedure patterns:

- `Get...`: fetch single records or detailed views
- `uspGetAll...`: paged/search list retrieval
- `InsertUpdate...` or `SaveUpdate...`: upsert-style write operations
- `Delete...`: delete or status-change operations
- `uspPreview...`: billing/wage previews
- `uspGenerate...`: billing/wage generation

Examples:

- `User/Stored Procedures/InsertUpdateUser.sql`
- `User/Stored Procedures/uspGetAllUsers.sql`
- `dbo/Stored Procedures/uspGetPlanboardTasks.sql`
- `dbo/Stored Procedures/uspGetToConfirmTasks.sql`
- `dbo/Stored Procedures/uspGetBillingInfo.sql`
- `dbo/Stored Procedures/uspGetWageInfo.sql`

## Cross-Project Relationship

The API repository layer depends directly on these procedures.

Typical backend flow:

1. API repository builds Dapper parameters
2. repository calls a stored procedure
3. procedure reads/writes tables
4. result is mapped back to API models

Because of that, DB procedure names and parameter contracts are part of the application interface and should be changed carefully.

## Build And Deployment

The SQL project file is `Scheduler.DB.sqlproj`.

Key characteristics:

- SQL Server database project
- builds a DACPAC
- includes tables, functions, and stored procedures as `Build` items
- includes `Script.SchedulerPostDeployment.sql` as a post-deploy script

Operational helper files in `Scripts/` may be included as `None` items instead of build items.

## Seed And Post-Deploy Data

`Script.SchedulerPostDeployment.sql` is used for post-deployment seeding and setup.

This is the right place to check when you need to understand:

- lookup seeds
- status IDs
- baseline system configuration

## Notable Operational Scripts

Examples in `Scripts/`:

- `CreateDelayedTaskStatusJob.sql`
- `DiagnosePreferenceLookupIssue.sql`
- `InsertPreferenceLookupData.sql`

These are useful for environment setup, troubleshooting, or scheduled job support.

## Important Recent Example

The project already contains:

- `dbo/Stored Procedures/uspMarkOverdueTasksAsDelayed.sql`
- `Scripts/CreateDelayedTaskStatusJob.sql`

This shows the intended pattern for background/status automation:

- business rule in a stored procedure
- scheduling/ops wiring in a separate helper script

## Suggested Database Reading Order

1. `Scheduler.DB.sqlproj`
2. `Script.SchedulerPostDeployment.sql`
3. key tables such as `tblUser`, `tblServicesTask`, `tblBillingInvoice`, `tblServiceProviderWage`
4. matching stored procedures such as `InsertUpdateUser`, `uspGetAllUsers`, `uspGetPlanboardTasks`, `uspGetBillingInfo`

## Change Guidelines

- when renaming or changing stored procedure parameters, update the API repository code in the same change
- keep new schema objects included in the `.sqlproj`
- prefer domain-appropriate folders to keep procedures discoverable
- add operational scripts to `Scripts/` when setup or scheduled jobs are needed outside the normal build

## Improvement Opportunities

- add a formal ERD
- document key foreign-key relationships
- document canonical status IDs and lookup groups
- add a mapping table between API repositories and stored procedures
