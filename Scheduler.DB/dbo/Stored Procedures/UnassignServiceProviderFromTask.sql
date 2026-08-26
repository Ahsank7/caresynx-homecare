-- =============================================
-- Description: Remove service provider from a task, set status to Unassigned, write task log.
-- =============================================
CREATE PROCEDURE [dbo].[UnassignServiceProviderFromTask]
    @pTaskId NVARCHAR(1000),
    @pUpdatedBy UNIQUEIDENTIFIER = NULL,
    @pNotes NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UnassignedStatusId INT;
    DECLARE @CompletedStatusId INT;
    DECLARE @CancelledStatusId INT;
    DECLARE @PreviousServiceProviderId UNIQUEIDENTIFIER;
    DECLARE @PreviousStatus INT;
    DECLARE @PreviousProviderName NVARCHAR(300);
    DECLARE @PreviousStatusName NVARCHAR(100);
    DECLARE @UnassignedName NVARCHAR(100);

    SELECT @UnassignedStatusId = Id
    FROM [dbo].[tblLookupItems]
    WHERE LookupType = 'TaskStatus' AND Name = 'Unassigned';

    SELECT @CompletedStatusId = Id
    FROM [dbo].[tblLookupItems]
    WHERE LookupType = 'TaskStatus' AND Name = 'Completed';

    SELECT @CancelledStatusId = Id
    FROM [dbo].[tblLookupItems]
    WHERE LookupType = 'TaskStatus' AND Name = 'Cancelled';

    IF @UnassignedStatusId IS NULL
    BEGIN
        RAISERROR('TaskStatus Unassigned is not defined in tblLookupItems.', 16, 1);
        RETURN;
    END

    SELECT
        @PreviousServiceProviderId = st.ServiceProviderId,
        @PreviousStatus = st.[Status]
    FROM [dbo].[tblServicesTask] st
    WHERE st.Id = CAST(@pTaskId AS INT);

    IF @PreviousServiceProviderId IS NULL
    BEGIN
        RAISERROR('This task has no assigned service provider.', 16, 1);
        RETURN;
    END

    IF @PreviousStatus IN (@CompletedStatusId, @CancelledStatusId)
    BEGIN
        RAISERROR('Cannot unassign a task that is Completed or Cancelled.', 16, 1);
        RETURN;
    END

    SELECT @PreviousProviderName = LTRIM(RTRIM(ISNULL(su.FirstName, '') + ' ' + ISNULL(su.SurName, '') + ' ' + ISNULL(su.LastName, '')))
    FROM [dbo].[tblUser] su
    WHERE su.Id = @PreviousServiceProviderId;

    SELECT @PreviousStatusName = Name FROM [dbo].[tblLookupItems] WHERE LookupType = 'TaskStatus' AND Id = @PreviousStatus;
    SELECT @UnassignedName = Name FROM [dbo].[tblLookupItems] WHERE LookupType = 'TaskStatus' AND Id = @UnassignedStatusId;

    UPDATE [dbo].[tblServicesTask]
    SET
        ServiceProviderId = NULL,
        [Status] = @UnassignedStatusId,
        UpdatedBy = @pUpdatedBy,
        UpdatedDate = GETDATE(),
        Notes = CASE WHEN @pNotes IS NOT NULL THEN @pNotes ELSE Notes END
    WHERE Id = CAST(@pTaskId AS INT);

    INSERT INTO [dbo].[tblTaskLog]
    (
        [TaskId],
        [ActionType],
        [PreviousValue],
        [NewValue],
        [FieldName],
        [Description],
        [CreatedBy]
    )
    VALUES
    (
        CAST(@pTaskId AS INT),
        'ServiceProviderUnassign',
        CAST(@PreviousServiceProviderId AS NVARCHAR(50)) + ' (' + ISNULL(@PreviousProviderName, 'Unknown') + ')',
        N'Unassigned',
        N'ServiceProvider',
        N'Task was assigned to ' + ISNULL(@PreviousProviderName, 'Unknown') + N'; it is now unassigned and may be assigned to someone else later.',
        @pUpdatedBy
    );

    IF @PreviousStatus <> @UnassignedStatusId
    BEGIN
        INSERT INTO [dbo].[tblTaskLog]
        (
            [TaskId],
            [ActionType],
            [PreviousValue],
            [NewValue],
            [FieldName],
            [Description],
            [CreatedBy]
        )
        VALUES
        (
            CAST(@pTaskId AS INT),
            N'StatusChange',
            CAST(@PreviousStatus AS NVARCHAR(10)),
            CAST(@UnassignedStatusId AS NVARCHAR(10)) + N' (' + ISNULL(@UnassignedName, N'Unassigned') + N')',
            N'Status',
            N'Task status changed from ' + ISNULL(@PreviousStatusName, CAST(@PreviousStatus AS NVARCHAR(10))) + N' to ' + ISNULL(@UnassignedName, N'Unassigned') + N' (unassigned).',
            @pUpdatedBy
        );
    END
END
