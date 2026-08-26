-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

CREATE PROCEDURE [dbo].[UpdateTaskAttendance] --[CLIENT].[DeleteClient] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pTaskId int,
	@pTaskTime DateTime,
	@pUpdatedBy uniqueidentifier = NULL
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	declare @vCheckInTime datetime
	declare @vCheckOutTime datetime
	declare @vPreviousStatus int
	DECLARE @vPreviousStatusName nvarchar(50)
	declare @vNewStatus int
	declare @vStatusName nvarchar(50)
	
	select top 1 @vCheckInTime=CheckIn, @vCheckOutTime=CheckOut, @vPreviousStatus=Status, @vPreviousStatusName = (Select top 1 LI.Name FROM tblLookupItems LI WHERE LookupType ='TaskStatus' and Id=[Status] )
	from tblServicesTask where id=@pTaskId
	
	if @vCheckInTime is null 
	begin
		-- Update CheckIn time and status to In-Progress
		Update [dbo].[tblServicesTask]
		set CheckIn=@pTaskTime,
		[Status] = (select top 1 Id from tblLookupItems where LookupType ='TaskStatus' and [Name]='In-Progress')  
		where id=@pTaskId
		
		-- Get new status for logging
		SELECT @vNewStatus = Id, @vStatusName = Name 
		FROM tblLookupItems 
		WHERE LookupType ='TaskStatus' and [Name]='In-Progress'
		
		-- Log the check-in
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
			NULL,
			FORMAT(@pTaskTime, 'yyyy-MM-dd hh:mm tt'),
			'CheckIn',
			'Task check-in time recorded',
			@pUpdatedBy
		);
		
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
				@vPreviousStatusName,
				' (' + @vStatusName + ')',
				'Status',
				'Task status changed to In-Progress due to check-in',
				@pUpdatedBy
			);
		END
	end
	else if @vCheckOutTime is null
	begin
		-- Update CheckOut time and status to Completed
		Update [dbo].[tblServicesTask]
		set CheckOut=@pTaskTime,
		[Status] = (select top 1 Id from tblLookupItems where LookupType ='TaskStatus' and [Name]='Completed')  
		where id=@pTaskId
		
		-- Get new status for logging
		SELECT @vNewStatus = Id, @vStatusName = Name 
		FROM tblLookupItems 
		WHERE LookupType ='TaskStatus' and [Name]='Completed'
		
		-- Log the check-out
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
			NULL,
			FORMAT(@pTaskTime, 'yyyy-MM-dd hh:mm tt'),
			'CheckOut',
			'Task check-out time recorded',
			@pUpdatedBy
		);
		
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
				@vPreviousStatusName,
				' (' + @vStatusName + ')',
				'Status',
				'Task status changed to Completed due to check-out',
				@pUpdatedBy
			);
		END
	end
	
END