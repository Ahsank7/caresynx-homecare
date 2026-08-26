-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

CREATE PROCEDURE [dbo].[UpdateTaskNotes] --[CLIENT].[DeleteClient] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pTaskId nvarchar(1000),
	@pTaskNotes nvarchar(500)='',
	@pUpdatedBy uniqueidentifier = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @PreviousNotes NVARCHAR(500);
	
	-- Get previous notes for logging
	SELECT @PreviousNotes = Notes 
	FROM [dbo].[tblServicesTask] 
	WHERE Id = @pTaskId;
	
	-- Update the task notes
	Update [dbo].[tblServicesTask]
	Set Notes = @pTaskNotes
	Where Id = @pTaskId;
	
	-- Log the change if notes were actually changed
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
			'Task notes updated',
			@pUpdatedBy
		);
	END
END