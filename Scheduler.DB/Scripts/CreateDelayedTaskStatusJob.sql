/*
    Creates a SQL Server Agent job that marks overdue scheduled tasks as Delayed.

    Important:
    - This script is intended for SQL Server / Managed Instance environments that support SQL Server Agent.
    - If the database is deployed to Azure SQL Database (single database), SQL Agent is not available.
      In that case, run dbo.uspMarkOverdueTasksAsDelayed from Elastic Jobs, Azure Automation, or another scheduler.
*/

USE [msdb];
GO

DECLARE @JobName SYSNAME = N'Scheduler - Mark Overdue Tasks As Delayed';
DECLARE @DatabaseName SYSNAME = N'SchedulerDB';
DECLARE @JobId UNIQUEIDENTIFIER;

IF EXISTS (SELECT 1 FROM dbo.sysjobs WHERE name = @JobName)
BEGIN
    EXEC dbo.sp_delete_job @job_name = @JobName;
END
GO

DECLARE @JobName SYSNAME = N'Scheduler - Mark Overdue Tasks As Delayed';
DECLARE @DatabaseName SYSNAME = N'SchedulerDB';
DECLARE @JobId UNIQUEIDENTIFIER;

EXEC dbo.sp_add_job
    @job_name = @JobName,
    @enabled = 1,
    @description = N'Automatically updates overdue scheduled tasks to Delayed.',
    @start_step_id = 1,
    @job_id = @JobId OUTPUT;

EXEC dbo.sp_add_jobstep
    @job_id = @JobId,
    @step_id = 1,
    @step_name = N'Mark overdue tasks as delayed',
    @subsystem = N'TSQL',
    @database_name = @DatabaseName,
    @command = N'EXEC dbo.uspMarkOverdueTasksAsDelayed;',
    @on_success_action = 1,
    @on_fail_action = 2;

EXEC dbo.sp_add_schedule
    @schedule_name = N'Scheduler - Every 5 Minutes - Delayed Tasks',
    @enabled = 1,
    @freq_type = 4,
    @freq_interval = 1,
    @freq_subday_type = 4,
    @freq_subday_interval = 30,
    @active_start_time = 000000;

EXEC dbo.sp_attach_schedule
    @job_id = @JobId,
    @schedule_name = N'Scheduler - Every 5 Minutes - Delayed Tasks';

EXEC dbo.sp_add_jobserver
    @job_id = @JobId,
    @server_name = N'(local)';
GO
