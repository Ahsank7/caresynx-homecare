-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

CREATE PROCEDURE [dbo].[AddTaskAttendance] --[CLIENT].[DeleteClient] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pTaskId int,
	@pTaskCheckInTime DateTime,
	@pTaskCheckOutTime DateTime,
	@pUpdatedBy uniqueidentifier = NULL
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @vPreviousCheckIn datetime
	DECLARE @vPreviousCheckOut datetime
	DECLARE @vPreviousStatus int
	DECLARE @vNewStatus int
	DECLARE @vStatusName nvarchar(50)
	DECLARE @vPreviousStatusName nvarchar(50)
	
	-- Get previous values for logging
	SELECT @vPreviousCheckIn = CheckIn, @vPreviousCheckOut = CheckOut, @vPreviousStatus = Status, @vPreviousStatusName = (Select top 1 LI.Name FROM tblLookupItems LI WHERE LookupType ='TaskStatus' and Id=[Status] )
	FROM tblServicesTask WHERE id = @pTaskId
	
	-- Get new status for logging
	SELECT @vNewStatus = Id, @vStatusName = Name 
	FROM tblLookupItems 
	WHERE LookupType ='TaskStatus' and [Name]='Completed'

	-- Update the task attendance
	Update [dbo].[tblServicesTask]
	set CheckIn = @pTaskCheckInTime,
	    CheckOut = @pTaskCheckOutTime,
	    [Status] = @vNewStatus
	where id = @pTaskId
	
	-- Log the check-in time change
	IF (@vPreviousCheckIn IS NULL AND @pTaskCheckInTime IS NOT NULL) OR 
	   (@vPreviousCheckIn IS NOT NULL AND @pTaskCheckInTime IS NULL) OR
	   (@vPreviousCheckIn != @pTaskCheckInTime)
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
			'AttendanceUpdate',
			FORMAT(@vPreviousCheckIn, 'yyyy-MM-dd hh:mm tt'),
			FORMAT(@pTaskCheckInTime, 'yyyy-MM-dd hh:mm tt'),
			'CheckIn',
			'Task check-in time updated',
			@pUpdatedBy
		);
	END
	
	-- Log the check-out time change
	IF (@vPreviousCheckOut IS NULL AND @pTaskCheckOutTime IS NOT NULL) OR 
	   (@vPreviousCheckOut IS NOT NULL AND @pTaskCheckOutTime IS NULL) OR
	   (@vPreviousCheckOut != @pTaskCheckOutTime)
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
			'AttendanceUpdate',
			FORMAT(@vPreviousCheckIn, 'yyyy-MM-dd hh:mm tt'),
			FORMAT(@pTaskCheckInTime, 'yyyy-MM-dd hh:mm tt'),
			'CheckOut',
			'Task check-out time updated',
			@pUpdatedBy
		);
	END
	
	-- Log the status change
	IF @vPreviousStatusName != @vStatusName
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
			CAST(@vPreviousStatusName AS NVARCHAR(10)),
			' (' + @vStatusName + ')',
			'Status',
			'Task status changed to Completed due to attendance update',
			@pUpdatedBy
		);
	END
END