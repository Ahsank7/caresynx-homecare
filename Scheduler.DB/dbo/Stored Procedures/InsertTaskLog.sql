CREATE PROCEDURE [dbo].[InsertTaskLog]
    @pTaskId INT,
    @pActionType NVARCHAR(50),
    @pPreviousValue NVARCHAR(MAX) = NULL,
    @pNewValue NVARCHAR(MAX) = NULL,
    @pFieldName NVARCHAR(100) = NULL,
    @pDescription NVARCHAR(500) = NULL,
    @pCreatedBy UNIQUEIDENTIFIER,
    @pIPAddress NVARCHAR(45) = NULL,
    @pUserAgent NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [dbo].[tblTaskLog]
    (
        [TaskId],
        [ActionType],
        [PreviousValue],
        [NewValue],
        [FieldName],
        [Description],
        [CreatedBy],
        [IPAddress],
        [UserAgent]
    )
    VALUES
    (
        @pTaskId,
        @pActionType,
        @pPreviousValue,
        @pNewValue,
        @pFieldName,
        @pDescription,
        @pCreatedBy,
        @pIPAddress,
        @pUserAgent
    );
    
    SELECT SCOPE_IDENTITY() AS LogId;
END
