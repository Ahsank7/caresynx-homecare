--exec [dbo].[CreateSchedule] @pDescription=N'sfsd',@pStartTime='2024-10-29 20:14:00',@pEndTime='2024-11-28 21:14:00',@pRecurrencePattern=1,@pRecurrenceInterval=0,@pRecurrenceDaysOfWeek=N''
--,@pRecurrenceDayOfMonth=N'',@pRecurrenceMonthOfYear=N'',@pTimeZone=N'Pakistan Standard Time',@pServiceType=1036,@pCSVServiceIds=N'1037',@pClientId='13DBA672-50B4-42C2-9DA9-E25D1E881CF1',@pCSVServiceProviderIds=N'7fc767ba-68ca-44f2-95ba-53ae287b777a',@pCreatedBy='00000000-0000-0000-0000-000000000000'


CREATE PROCEDURE [dbo].[CreateSchedule]
    @pDescription NVARCHAR(100),
    @pStartTime DATETIME,
    @pEndTime DATETIME,
    @pRecurrencePattern INT = NULL,  -- Use INT to represent the recurrence pattern  1 -- 'Daily' -- 2 'Weekly' --3 -- 'Monthly' 4-'Yearly'
    @pRecurrenceInterval INT = NULL, -- gap 0,1
    @pRecurrenceDaysOfWeek NVARCHAR(50) = NULL,
    @pRecurrenceDayOfMonth NVARCHAR(MAX) = NULL,    
    @pRecurrenceMonthOfYear NVARCHAR(MAX) = NULL,
    @pServiceType INT,
    @pCSVServiceIds NVARCHAR(50),
    @pClientId UNIQUEIDENTIFIER,
    @pCSVServiceProviderIds NVARCHAR(MAX),
    @pCreatedBy UNIQUEIDENTIFIER=null,
	@pOrganizationId UNIQUEIDENTIFIER=null
AS
BEGIN
    SET NOCOUNT ON;

    -- Get timezone from organization settings
    DECLARE @pTimeZone NVARCHAR(100)
    SELECT @pTimeZone = [TimeZone] FROM [dbo].[tblOrganization] WHERE [Id] = @pOrganizationId AND [IsActive] = 1
    
    -- If no organization timezone found, use default
    If IsNull(@pTimeZone,'') = ''
      Set @pTimeZone='Pakistan Standard Time'
    
    -- Convert timezone name to SQL Server timezone identifier
    DECLARE @pTimeZoneId NVARCHAR(50)
    SET @pTimeZoneId = [dbo].[GetTimeZoneId](@pTimeZone)

    -- Validate input parameters
    IF @pStartTime IS NULL OR @pEndTime IS NULL
    BEGIN
        RAISERROR('Title, start time, and end time are required', 16, 1);
      RETURN;
    END

    -- Insert schedule details
    INSERT INTO [dbo].[tblScheduler]
    (
        [StartTime],
        [EndTime],
        [RecurrencePattern],
        [RecurrenceInterval],
        [RecurrenceDaysOfWeek],
        [RecurrenceDayOfMonth],
        [RecurrenceMonthOfYear],
        [ServiceType],
        [CSVServiceIds],
        [ClientId],
        [CSVServiceProviderIds],
        [Description],
        [CreatedDate],
        [CreatedBy],
		[TimeZone]
    )
    VALUES
    (
        @pStartTime,
        @pEndTime,
        @pRecurrencePattern,
        @pRecurrenceInterval,
        @pRecurrenceDaysOfWeek,
        @pRecurrenceDayOfMonth,
        @pRecurrenceMonthOfYear,
        @pServiceType,
        @pCSVServiceIds,
        @pClientId,
        @pCSVServiceProviderIds,
        @pDescription,
        GETDATE(),
        @pCreatedBy,
		@pTimeZone
    );

    -- Get the ID of the new schedule
    DECLARE @vScheduleId INT;
    SET @vScheduleId = SCOPE_IDENTITY();

    -- If a recurrence pattern is specified, calculate occurrence dates and insert them
    IF @pRecurrencePattern IS NOT NULL
    BEGIN

  WITH RecurrenceDates AS (
    SELECT TOP (DATEDIFF(DAY, @pStartTime, @pEndTime) + 1)
        OccurrenceDate = DATEADD(DAY, ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1, @pStartTime)
    FROM sys.all_objects
)
SELECT OccurrenceDate
INTO #TempOccurrences
FROM RecurrenceDates
WHERE
    (
        @pRecurrencePattern = 1 -- Daily
        OR
        (
            @pRecurrencePattern = 2 -- Weekly
            AND DATENAME(WEEKDAY, OccurrenceDate) IN (
                SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@pRecurrenceDaysOfWeek, ',')
            )
        )
        OR
        (
            @pRecurrencePattern = 3 -- Monthly
            AND CAST(DAY(OccurrenceDate) AS NVARCHAR(10)) IN (
                SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@pRecurrenceDayOfMonth, ',')
            )
        )
        OR
        (
            @pRecurrencePattern = 4 -- Yearly
            AND CAST(MONTH(OccurrenceDate) AS NVARCHAR(10)) IN (
                SELECT LTRIM(RTRIM(value)) FROM STRING_SPLIT(@pRecurrenceMonthOfYear, ',')
            )
            AND (ISNULL(@pRecurrenceInterval, 1) <> 0 OR DATEDIFF(DAY, @pStartTime, OccurrenceDate) % @pRecurrenceInterval = 0)
        )
    );


        -- Insert the calculated occurrence dates into the tblServicesTask table
        INSERT INTO [dbo].[tblServicesTask]
        (
            [ScheduleId],
            [EndTime],
            [Date],
            [StartTime],
            [ClientId],
            [ServiceProviderId],
            [CreatedDate],
            [CreatedBy],
            [Status],
            [Notes]
        )
        SELECT
            @vScheduleId,
            Cast( CONVERT(VARCHAR, OccurrenceDate, 23)+' '+FORMAT(@pEndTime, 'HH:mm:ss') as datetime),
            OccurrenceDate,
            Cast( CONVERT(VARCHAR, OccurrenceDate, 23)+' '+FORMAT(@pStartTime, 'HH:mm:ss') as datetime),
            @pClientId,
            value, -- Assuming ServiceProviderId is the same as ClientId for now
            GETDATE(),
            @pCreatedBy,
			(select top 1 Id from tblLookupItems  where LookupType='TaskStatus' and Name='Scheduled'),
            @pDescription
        FROM #TempOccurrences
		CROSS APPLY STRING_SPLIT(@pCSVServiceProviderIds, ',') -- Use STRING_SPLIT to split the CSV string

    END

    SELECT @vScheduleId;
END