--select * from tblLookupItems

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Assign a service provider to an unassigned task
-- =============================================

CREATE PROCEDURE [dbo].[AssignServiceProviderToTask]
	@pTaskId nvarchar(1000),
	@pServiceProviderId uniqueidentifier,
	@pUpdatedBy uniqueidentifier = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @PreviousServiceProviderId UNIQUEIDENTIFIER;
	DECLARE @PreviousStatus INT;
	DECLARE @ScheduledStatusId INT = 33; -- Scheduled status ID
	
	-- Get previous values for logging
	SELECT @PreviousServiceProviderId = ServiceProviderId, @PreviousStatus = Status
	FROM [dbo].[tblServicesTask] 
	WHERE Id = @pTaskId;
	
	-- Update the task with new service provider and set status to Scheduled
	UPDATE [dbo].[tblServicesTask]
	SET ServiceProviderId = @pServiceProviderId,
	    Status = @ScheduledStatusId,
		UpdatedBy = @pUpdatedBy,
		UpdatedDate = GETDATE()
	WHERE Id = @pTaskId;
	
	-- Log the service provider assignment
	IF @PreviousServiceProviderId != @pServiceProviderId OR @PreviousServiceProviderId IS NULL
	BEGIN
		DECLARE @PreviousProviderName NVARCHAR(200) = 'Unassigned';
		DECLARE @NewProviderName NVARCHAR(200);
		
		-- Get provider name if previous provider existed
		IF @PreviousServiceProviderId IS NOT NULL
		BEGIN
			SELECT @PreviousProviderName = FirstName + ' ' + LastName
			FROM [dbo].[tblUser]
			WHERE Id = @PreviousServiceProviderId;
		END
		
		-- Get new provider name
		SELECT @NewProviderName = FirstName + ' ' + LastName
		FROM [dbo].[tblUser]
		WHERE Id = @pServiceProviderId;
		
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
			'ServiceProviderAssignment',
			ISNULL(CAST(@PreviousServiceProviderId AS NVARCHAR(50)), 'Unassigned'),
			CAST(@pServiceProviderId AS NVARCHAR(50)) + ' (' + ISNULL(@NewProviderName, 'Unknown') + ')',
			'ServiceProvider',
			'Service provider assigned: ' + ISNULL(@PreviousProviderName, 'Unassigned') + ' -> ' + ISNULL(@NewProviderName, 'Unknown'),
			@pUpdatedBy
		);
	END
	
	-- Log the status change from Unassigned to Scheduled
	IF @PreviousStatus != @ScheduledStatusId
	BEGIN
		DECLARE @StatusName NVARCHAR(50);
		SELECT @StatusName = Name 
		FROM [dbo].[tblLookupItems] 
		WHERE LookupType = 'TaskStatus' AND Id = @ScheduledStatusId;
		
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
			CAST(@ScheduledStatusId AS NVARCHAR(10)) + ' (' + ISNULL(@StatusName, 'Scheduled') + ')',
			'Status',
			'Task status changed from ' + CAST(@PreviousStatus AS NVARCHAR(10)) + ' to ' + CAST(@ScheduledStatusId AS NVARCHAR(10)) + ' (Scheduled)',
			@pUpdatedBy
		);
	END
END