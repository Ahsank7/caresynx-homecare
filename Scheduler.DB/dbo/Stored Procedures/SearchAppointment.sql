CREATE PROCEDURE [dbo].[SearchAppointment]  --SearchAppointment  '2023-03-28','2023-03-28','114BB6A3-812E-434C-9278-579AAD0E8361','7554CBBD-500D-4AF8-8367-592661D1E83E'
   @pStartDate DATE,
   @pEndDate DATE,
   @pClientId  uniqueidentifier,
   @pCSVServiceProviderId uniqueidentifier,
   @pStatusIds nvarchar(MAX)=null,
   @pOrganizationId UNIQUEIDENTIFIER=null

AS
BEGIN
   SET NOCOUNT ON;

   -- Get timezone from organization settings
   declare @pTimeZone nvarchar(500)
   SELECT @pTimeZone = [TimeZone] FROM [dbo].[tblOrganization] WHERE [Id] = @pOrganizationId AND [IsActive] = 1
   
   -- If no organization timezone found, use default
   If IsNull(@pTimeZone,'') = ''
     Set @pTimeZone='Pakistan Standard Time'
   
   -- Convert timezone name to SQL Server timezone identifier
   DECLARE @pTimeZoneId NVARCHAR(50)
   SET @pTimeZoneId = [dbo].[GetTimeZoneId](@pTimeZone)

   -- Validate input parameters
   IF  @pStartDate IS NULL OR @pEndDate IS NULL
   BEGIN
      RAISERROR('Title, start time, and end time are required', 16, 1);
      RETURN;
   END

  
	  SELECT ST.[ScheduleId]
           ,ST.[Date]
           ,CAST(ST.[StartTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) as StartTime
		   ,CAST(ST.[EndTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) as EndTime
           ,ST.[ClientId]
		   ,CU.FirstName
           ,ST.[ServiceProviderId]
		   ,SPU.FirstName
           ,ST.[CreatedDate]
           ,ST.[CreatedBy]
           ,ST.[Status]
           ,ST.[Notes]
      FROM [dbo].[tblServicesTask] ST 
	  JOIN dbo.tblClient C on ST.ClientId=C.Id  
	  JOIN dbo.tblServiceProvider SP on ST.ServiceProviderId=SP.Id
	  JOIN dbo.tblUser CU on CU.Id=C.UserId
	  JOIN dbo.tblUser SPU on SPU.Id=SP.UserId
	  Where 1=1
	    and ST.ClientId=@pClientId
		and ST.ServiceProviderId=@pCSVServiceProviderId
		and ST.[Date] between @pStartDate and @pEndDate
		--and ST.[Status] in (1,2)

	  --select * from tblTaskStatus

end