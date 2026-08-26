CREATE PROCEDURE [dbo].[uspMarkOverdueTasksAsDelayed]
    @pCurrentDateTime DATETIME = NULL,
    @pUpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentDateTime DATETIME = ISNULL(@pCurrentDateTime, GETDATE());
    DECLARE @ScheduledStatusId INT;
    DECLARE @DelayedStatusId INT;

    SELECT @ScheduledStatusId = Id
    FROM dbo.tblLookupItems
    WHERE LookupType = 'TaskStatus'
      AND Name = 'Scheduled';

    SELECT @DelayedStatusId = Id
    FROM dbo.tblLookupItems
    WHERE LookupType = 'TaskStatus'
      AND Name = 'Delayed';

    IF @ScheduledStatusId IS NULL
    BEGIN
        RAISERROR('Scheduled task status was not found in tblLookupItems.', 16, 1);
        RETURN;
    END

    IF @DelayedStatusId IS NULL
    BEGIN
        RAISERROR('Delayed task status was not found in tblLookupItems.', 16, 1);
        RETURN;
    END

    DECLARE @UpdatedTasks TABLE
    (
        TaskId INT NOT NULL,
        PreviousStatus INT NULL,
        NewStatus INT NOT NULL,
        PreviousNotes NVARCHAR(500) NULL,
        NewNotes NVARCHAR(500) NULL
    );

    UPDATE st
    SET
        st.Status = @DelayedStatusId,
        st.Notes = CASE
            WHEN ISNULL(LTRIM(RTRIM(st.Notes)), '') = '' THEN 'Task automatically marked as delayed because the start time passed without check-in.'
            ELSE st.Notes
        END,
        st.UpdatedDate = @CurrentDateTime,
        st.UpdatedBy = COALESCE(@pUpdatedBy, st.UpdatedBy)
    OUTPUT
        inserted.Id,
        deleted.Status,
        inserted.Status,
        deleted.Notes,
        inserted.Notes
    INTO @UpdatedTasks (TaskId, PreviousStatus, NewStatus, PreviousNotes, NewNotes)
    FROM dbo.tblServicesTask st
    WHERE st.Status = @ScheduledStatusId
      AND st.StartTime IS NOT NULL
      AND st.StartTime < @CurrentDateTime
      AND st.CheckIn IS NULL
      AND st.CheckOut IS NULL;

    INSERT INTO dbo.tblTaskLog
    (
        TaskId,
        ActionType,
        PreviousValue,
        NewValue,
        FieldName,
        Description,
        CreatedBy
    )
    SELECT
        ut.TaskId,
        'StatusChange',
        CAST(ut.PreviousStatus AS NVARCHAR(10)),
        CAST(ut.NewStatus AS NVARCHAR(10)) + ' (Delayed)',
        'Status',
        'Task status changed automatically from Scheduled to Delayed because the start time passed without check-in.',
        @pUpdatedBy
    FROM @UpdatedTasks ut;

    INSERT INTO dbo.tblTaskLog
    (
        TaskId,
        ActionType,
        PreviousValue,
        NewValue,
        FieldName,
        Description,
        CreatedBy
    )
    SELECT
        ut.TaskId,
        'NotesUpdate',
        ut.PreviousNotes,
        ut.NewNotes,
        'Notes',
        'Task notes updated during automatic delayed status processing.',
        @pUpdatedBy
    FROM @UpdatedTasks ut
    WHERE ISNULL(ut.PreviousNotes, '') <> ISNULL(ut.NewNotes, '');

    SELECT COUNT(1) AS UpdatedTaskCount
    FROM @UpdatedTasks;
END
