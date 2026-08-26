create PROCEDURE [dbo].[uspCalculateBillingAndWageAmounts_backup]
    @pServicesTasks NVARCHAR(MAX),
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Input validation
    IF @pServicesTasks IS NULL OR LEN(TRIM(@pServicesTasks)) = 0
    BEGIN
        RETURN;
    END

    -- Use table variable instead of temp table for small datasets (better performance)
    DECLARE @ServiceTaskIds TABLE (ServiceTaskId int PRIMARY KEY);

    -- Parse service task IDs efficiently
    INSERT INTO @ServiceTaskIds (ServiceTaskId)
    SELECT CAST(value AS int)
    FROM STRING_SPLIT(@pServicesTasks, ',')
    WHERE value IS NOT NULL AND LEN(TRIM(value)) > 0;

    -- Get organization settings with error handling
    DECLARE @CalculationTypeId INT;
    DECLARE @DefaultBillingRate DECIMAL(18, 2);
    DECLARE @DefaultWageRate DECIMAL(18, 2);
    DECLARE @UseServiceRateForBilling BIT;
    DECLARE @pTimeZoneId NVARCHAR(50);

    SELECT 
        @CalculationTypeId = CalculationTypeId,
        @DefaultBillingRate = DefaultBillingRate,
        @DefaultWageRate = DefaultWageRate,
        @UseServiceRateForBilling = ISNULL(UseServiceRateForBilling, 0),
        @pTimeZoneId = [dbo].[GetTimeZoneId](ISNULL(TimeZone, 'Pakistan Standard Time'))
    FROM [dbo].[tblOrganization]
    WHERE Id = @pOrganizationId;

    -- Single query to calculate and update everything at once
    -- This eliminates the need for temporary tables entirely
    WITH ServiceTaskCalculations AS (
        SELECT 
            st.Id AS ServiceTaskId,
            st.ScheduleId,
            st.ServiceProviderId,
            st.StartTime,
            st.EndTime,
            st.CheckIn,
            st.CheckOut,
            -- Pre-calculate timezone conversions
            CASE 
                WHEN @CalculationTypeId = 1 AND st.StartTime IS NOT NULL AND st.EndTime IS NOT NULL THEN
                    CAST(st.StartTime AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME)
                WHEN @CalculationTypeId = 2 AND st.CheckIn IS NOT NULL AND st.CheckOut IS NOT NULL THEN
                    CAST(st.CheckIn AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME)
                ELSE NULL
            END AS StartTimeConverted,
            CASE 
                WHEN @CalculationTypeId = 1 AND st.StartTime IS NOT NULL AND st.EndTime IS NOT NULL THEN
                    CAST(st.EndTime AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME)
                WHEN @CalculationTypeId = 2 AND st.CheckIn IS NOT NULL AND st.CheckOut IS NOT NULL THEN
                    CAST(st.CheckOut AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME)
                ELSE NULL
            END AS EndTimeConverted,
            -- Calculate billing rate
            CASE 
                WHEN @UseServiceRateForBilling = 1 AND ISNULL(s.Rate, 0) > 0 THEN s.Rate
                ELSE @DefaultBillingRate
            END AS BillingRate,
            -- Calculate wage rate
            CASE 
                WHEN spc.OptionId = 1 THEN ISNULL(spc.Rate, @DefaultWageRate)
                ELSE @DefaultWageRate
            END AS WageRate,
            ISNULL(spc.OptionId, 0) AS WageOptionId
        FROM [dbo].[tblServicesTask] st
        INNER JOIN @ServiceTaskIds sti ON st.Id = sti.ServiceTaskId
        INNER JOIN [dbo].[tblScheduler] sch ON st.ScheduleId = sch.Id
        LEFT JOIN [dbo].[tblServices] s ON 
            sch.CSVServiceIds IS NOT NULL 
            AND LEN(TRIM(sch.CSVServiceIds)) > 0
            AND CHARINDEX(',' + CAST(s.Id AS NVARCHAR(10)) + ',', ',' + sch.CSVServiceIds + ',') > 0
        LEFT JOIN [dbo].[tblServiceProviderContract] spc ON 
            st.ServiceProviderId = spc.ServiceProviderUserId 
            AND spc.isActive = 1
    ),
    HoursCalculated AS (
        SELECT 
            ServiceTaskId,
            BillingRate,
            WageRate,
            WageOptionId,
            CASE 
                WHEN StartTimeConverted IS NULL OR EndTimeConverted IS NULL THEN 0
                WHEN EndTimeConverted < StartTimeConverted THEN 
                    DATEDIFF(HOUR, StartTimeConverted, EndTimeConverted + 1)
                ELSE 
                    DATEDIFF(HOUR, StartTimeConverted, EndTimeConverted)
            END AS HoursWorked
        FROM ServiceTaskCalculations
    )
    -- Single update statement with all calculations
    UPDATE st
    SET 
        st.BillingAmount = hc.BillingRate * hc.HoursWorked,
        st.BillingRate = hc.BillingRate,
        st.WageAmount = hc.WageRate * hc.HoursWorked,
        st.WageRate = hc.WageRate,
        st.WageOptionId = hc.WageOptionId,
        st.CalculationTypeId = @CalculationTypeId,
        st.IsConfirmed = 1
    FROM [dbo].[tblServicesTask] st
    INNER JOIN HoursCalculated hc ON st.Id = hc.ServiceTaskId;

END;

-- RECOMMENDED INDEXES FOR OPTIMAL PERFORMANCE:
/*
-- Index for tblServicesTask lookups
CREATE NONCLUSTERED INDEX IX_tblServicesTask_ServiceProviderId_ScheduleId 
ON [dbo].[tblServicesTask] (ServiceProviderId, ScheduleId) 
INCLUDE (Id, StartTime, EndTime, CheckIn, CheckOut);

-- Index for tblScheduler CSV lookups
CREATE NONCLUSTERED INDEX IX_tblScheduler_CSVServiceIds 
ON [dbo].[tblScheduler] (Id) 
INCLUDE (CSVServiceIds)
WHERE CSVServiceIds IS NOT NULL AND LEN(TRIM(CSVServiceIds)) > 0;

-- Index for tblServices rate lookups
CREATE NONCLUSTERED INDEX IX_tblServices_Rate 
ON [dbo].[tblServices] (Id) 
INCLUDE (Rate)
WHERE Rate IS NOT NULL AND Rate > 0;

-- Index for tblServiceProviderContract lookups
CREATE NONCLUSTERED INDEX IX_tblServiceProviderContract_ServiceProviderUserId_Active 
ON [dbo].[tblServiceProviderContract] (ServiceProviderUserId, isActive) 
INCLUDE (Rate, OptionId)
WHERE isActive = 1;

-- Index for tblOrganization lookups
CREATE NONCLUSTERED INDEX IX_tblOrganization_Id 
ON [dbo].[tblOrganization] (Id) 
INCLUDE (CalculationTypeId, DefaultBillingRate, DefaultWageRate, UseServiceRateForBilling, TimeZone);
*/