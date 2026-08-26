
CREATE PROCEDURE [dbo].[CreateScheduleUpdated]
    @pDescription NVARCHAR(100),
    @pStartTime DATETIME,
    @pEndTime DATETIME,
    @pRecurrencePattern INT = NULL,  -- Use INT to represent the recurrence pattern  1 -- 'Daily' -- 2 'Weekly' --3 -- 'Monthly' 4-'Yearly'
    @pRecurrenceInterval INT = NULL,
    @pRecurrenceDaysOfWeek NVARCHAR(50) = NULL,
    @pRecurrenceDayOfMonth NVARCHAR(MAX) = NULL,    
    @pRecurrenceMonthOfYear NVARCHAR(MAX) = NULL,

    @pServiceType INT,
    @pCSVServiceIds NVARCHAR(50),
    @pClientId UNIQUEIDENTIFIER,
    @pCSVServiceProviderIds NVARCHAR(MAX),
    @pCreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

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
        [CreatedBy]
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
        @pCreatedBy
    );

    -- Get the ID of the new schedule
    DECLARE @vScheduleId INT;
    SET @vScheduleId = SCOPE_IDENTITY();

    -- If a recurrence pattern is specified, calculate occurrence dates and insert them
    IF @pRecurrencePattern IS NOT NULL
    BEGIN
        DECLARE @Occurrences TABLE (OccurrenceDate DATETIME);
        DECLARE @vOccurrenceDate DATE = @pStartTime;
		DECLARE @vEndDate DATE = CAST(@pEndTime AS DATE);


        WHILE @vOccurrenceDate <= @vEndDate
        BEGIN
            IF (@pRecurrencePattern = 1 -- 'Daily'
                OR (@pRecurrencePattern = 2 AND CHARINDEX(DATENAME(weekday, @vOccurrenceDate), @pRecurrenceDaysOfWeek) > 0) -- 'Weekly'
                OR (@pRecurrencePattern = 3 AND (CHARINDEX(CAST(DAY(@vOccurrenceDate) AS NVARCHAR(MAX)), @pRecurrenceDayOfMonth) > 0)) -- 'Monthly'
                OR (@pRecurrencePattern = 4 AND MONTH(@vOccurrenceDate) = ISNULL(@pRecurrenceMonthOfYear, MONTH(@pStartTime)) AND (CHARINDEX(CAST(MONTH(@vOccurrenceDate) AS NVARCHAR(MAX)), @pRecurrenceMonthOfYear) > 0))) -- 'Yearly'
            BEGIN
                INSERT INTO @Occurrences (OccurrenceDate) VALUES (@vOccurrenceDate);
            END

            SET @vOccurrenceDate = DATEADD(day, ISNULL(@pRecurrenceInterval, 1), @vOccurrenceDate);
        END

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
            @pEndTime,
            OccurrenceDate,
            @pStartTime,
            @pClientId,
            value, -- Assuming ServiceProviderId is the same as ClientId for now
            GETDATE(),
            @pCreatedBy,
            1, -- Status: 1 for active, adjust as needed
            @pDescription
        FROM @Occurrences
		CROSS APPLY STRING_SPLIT(@pCSVServiceProviderIds, ',') -- Use STRING_SPLIT to split the CSV string

    END

    SELECT @vScheduleId;
END