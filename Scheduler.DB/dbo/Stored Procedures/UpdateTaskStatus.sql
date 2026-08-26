-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

CREATE PROCEDURE [dbo].[UpdateTaskStatus] --[CLIENT].[DeleteClient] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pTaskId nvarchar(1000),
	@pTaskStatus int,
	@pTaskNotes nvarchar(500)=null,
	@pUpdatedBy uniqueidentifier = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @PreviousStatus INT;
	DECLARE @PreviousNotes NVARCHAR(500);
	DECLARE @StatusName NVARCHAR(50);
	
	-- Get previous values for logging
	SELECT @PreviousStatus = Status, @PreviousNotes = Notes
	FROM [dbo].[tblServicesTask] 
	WHERE Id = @pTaskId;
	
	-- Get status name for logging
	SELECT @StatusName = Name 
	FROM [dbo].[tblLookupItems] 
	WHERE LookupType = 'TaskStatus' AND Id = @pTaskStatus;
	
	-- Update the task status and notes
	Update [dbo].[tblServicesTask]
	Set Status = @pTaskStatus,
	    Notes = @pTaskNotes
	Where Id = @pTaskId;
	
	-- Log the status change
	IF @PreviousStatus != @pTaskStatus
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
			@pTaskId,
			'StatusChange',
			CAST(@PreviousStatus AS NVARCHAR(10)),
			CAST(@pTaskStatus AS NVARCHAR(10)) + ' (' + ISNULL(@StatusName, 'Unknown') + ')',
			'Status',
			'Task status changed from ' + CAST(@PreviousStatus AS NVARCHAR(10)) + ' to ' + CAST(@pTaskStatus AS NVARCHAR(10)),
			@pUpdatedBy
		);
	END
	
	-- Log the notes change if notes were actually changed
	IF (@PreviousNotes IS NULL AND @pTaskNotes IS NOT NULL) OR 
	   (@PreviousNotes IS NOT NULL AND @pTaskNotes IS NULL) OR
	   (@PreviousNotes != @pTaskNotes)
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
			@pTaskId,
			'NotesUpdate',
			@PreviousNotes,
			@pTaskNotes,
			'Notes',
			'Task notes updated with status change',
			@pUpdatedBy
		);
	END
END