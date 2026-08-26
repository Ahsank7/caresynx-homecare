CREATE PROCEDURE [dbo].[GetClientTasks]
    -- Add the parameters for the stored procedure here
    @pStartDate DATETIME,
    @pEndDate DATETIME,
    @pClientId UNIQUEIDENTIFIER,
    @pStatusIds NVARCHAR(50)=null,
	@pOrganizationId UNIQUEIDENTIFIER=null
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

	-- Get timezone from organization settings
	DECLARE @pTimeZone NVARCHAR(500)
	SELECT @pTimeZone = [TimeZone] FROM [dbo].[tblOrganization] WHERE [Id] = @pOrganizationId AND [IsActive] = 1
	
	-- If no organization timezone found, use default
	If IsNull(@pTimeZone,'') = ''
	  Set @pTimeZone='Pakistan Standard Time'
	
	-- Convert timezone name to SQL Server timezone identifier
	DECLARE @pTimeZoneId NVARCHAR(50)
	SET @pTimeZoneId = [dbo].[GetTimeZoneId](@pTimeZone)

	 If IsNull(@pStatusIds,'') = ''
	  Set @pStatusIds=null

	declare @vStartDate date=Cast(@pStartDate as date)
	declare @vEndDate date=Cast(@pEndDate as date)
	--declare @vStartTime time=Cast(@pStartDate as time)
	--declare @vEndTime time=Cast(@pEndDate as time)

	--print @vStartTime
	--print @vEndTime
    SELECT 
        St.[Id] AS TaskId,
		ST.[Date],
        St.[Status] AS TaskStatusId,
        ST.[ClientId],
		CU.FirstName+' '+CU.LastName as ClientFullName,
        ST.[ServiceProviderId],
		SPU.FirstName+' '+SPU.LastName as ServiceProviderFullName,
		T.[Name] as TaskStatus,
        St.[ScheduleId],
		    -- Convert StartTime only as TIME, then add back to original Date
    DATEADD(
        DAY, 0,
        CAST(St.[Date] AS datetime) +
        CAST(
            CAST(St.[StartTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS time)
            AS datetime
        )
    ) AS StartTime,

    DATEADD(
        DAY, 0,
        CAST(St.[Date] AS datetime) +
        CAST(
            CAST(St.[EndTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS time)
            AS datetime
        )
    ) AS EndTime,
        st.[Status],
        ISNULL(sty.[Name], 'N/A') as ServiceType,
        ISNULL(s.[Name], 'N/A') as ServiceName
      
    FROM [dbo].[tblServicesTask] AS St
	  JOIN dbo.tblUser CU on CU.Id=ST.ClientId
	  JOIN dbo.tblUser SPU on SPU.Id=ST.ServiceProviderId
	  LEFT join [tblLookupItems] T on T.LookupType='TaskStatus' and T.Id=st.[Status]
	  LEFT JOIN [dbo].[tblScheduler] (Nolock) sch on sch.Id = st.ScheduleId
	  LEFT JOIN [dbo].[tblServicesType] (Nolock) sty on sty.Id = sch.ServiceType
	  LEFT JOIN [dbo].[tblServices] (Nolock) s on s.Id = sch.CSVServiceIds
    WHERE 1=1 
	    AND (ST.[Date] between @vStartDate and @vEndDate)
	    --AND (Cast(ST.StartTime AS Time) BETWEEN @vStartTime AND @vEndTime
     --           OR Cast(ST.EndTime AS Time) BETWEEN @vStartTime AND @vEndTime)
        AND St.[ClientId] = @pClientId
        AND St.[ServiceProviderId] IS NOT NULL
        AND (@pStatusIds is null OR St.[Status] IN (SELECT value FROM string_split(@pStatusIds,',')))
END