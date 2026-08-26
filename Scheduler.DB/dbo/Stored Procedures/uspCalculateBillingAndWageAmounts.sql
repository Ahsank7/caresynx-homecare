CREATE PROCEDURE [dbo].[uspCalculateBillingAndWageAmounts]
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
    DECLARE @ServiceRateForBilling INT; -- Changed from UseServiceRateForBilling BIT
    DECLARE @pTimeZoneId NVARCHAR(50);

    SELECT 
        @CalculationTypeId = CalculationTypeId,
        @DefaultBillingRate = DefaultBillingRate,
        @DefaultWageRate = DefaultWageRate,
        @ServiceRateForBilling = ISNULL(ServiceRateForBilling, 1), -- 1=Default, 2=Service-Specific, 3=Time-Based
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
			st.[Date],
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
            -- Get service information for time-based rate lookup
            s.Id AS ServiceId,
            s.ServiceTypeId,
            -- Calculate billing rate based on ServiceRateForBilling
            CASE 
                WHEN @ServiceRateForBilling = 1 THEN @DefaultBillingRate -- Default Rate
                WHEN @ServiceRateForBilling = 2 AND ISNULL(s.Rate, 0) > 0 THEN s.Rate -- Service-Specific Rate
                WHEN @ServiceRateForBilling = 3 THEN 
                    -- Time-Based Rate - will be calculated in the next CTE
                    NULL -- Placeholder, will be calculated based on time-based rates
                ELSE @DefaultBillingRate
            END AS BillingRate,
            -- Calculate wage rate
            CASE 
                WHEN spc.OptionId = 1 AND ISNULL(spc.Rate, 0) > 0 THEN spc.Rate
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
    TimeBasedRateCalculations AS (
        SELECT 
            stc.ServiceTaskId,
            stc.BillingRate,
            stc.WageRate,
            stc.WageOptionId,
            stc.StartTimeConverted,
            stc.EndTimeConverted,
            stc.ServiceId,
            stc.ServiceTypeId,
            -- Calculate time-based billing rate
            CASE 
                WHEN @ServiceRateForBilling = 3 AND stc.StartTimeConverted IS NOT NULL AND stc.EndTimeConverted IS NOT NULL THEN
                    -- Look for matching time-based rate
                    ISNULL((
                        SELECT TOP 1 otbr.ClientRate
                        FROM [dbo].[tblOrganizationTimeBasedRates] otbr
                        WHERE otbr.OrganizationId = @pOrganizationId
                        AND otbr.IsActive = 1
                        AND (
                            -- Match by service type and service (most specific)
                            (otbr.ServiceTypeId = stc.ServiceTypeId AND otbr.ServiceId = stc.ServiceId)
                            OR
                            -- Match by service type only (less specific)
                            (otbr.ServiceTypeId = stc.ServiceTypeId AND otbr.ServiceId IS NULL)
                            OR
                            -- Match by service only (less specific)
                            (otbr.ServiceTypeId IS NULL AND otbr.ServiceId = stc.ServiceId)
                            OR
                            -- Match all services (least specific)
                            (otbr.ServiceTypeId IS NULL AND otbr.ServiceId IS NULL)
                        )
                        AND otbr.DayOfWeek = DATEPART(WEEKDAY, stc.[Date]) - 1 -- Convert to 0-6 format (Sunday=0)
                        AND CAST(stc.StartTimeConverted AS TIME) >= otbr.StartTime
                        AND CAST(stc.StartTimeConverted AS TIME) < otbr.EndTime
                        ORDER BY 
                            -- Priority: service type + service > service type only > service only > all services
                            CASE WHEN otbr.ServiceTypeId IS NOT NULL AND otbr.ServiceId IS NOT NULL THEN 1
                                 WHEN otbr.ServiceTypeId IS NOT NULL AND otbr.ServiceId IS NULL THEN 2
                                 WHEN otbr.ServiceTypeId IS NULL AND otbr.ServiceId IS NOT NULL THEN 3
                                 ELSE 4 END
                    ), @DefaultBillingRate)
                ELSE stc.BillingRate
            END AS FinalBillingRate,
            -- Calculate time-based wage rate
            CASE 
                WHEN @ServiceRateForBilling = 3 AND stc.StartTimeConverted IS NOT NULL AND stc.EndTimeConverted IS NOT NULL THEN
                    -- Look for matching time-based wage rate
                    ISNULL((
                        SELECT TOP 1 otbr.WageRate
                        FROM [dbo].[tblOrganizationTimeBasedRates] otbr
                        WHERE otbr.OrganizationId = @pOrganizationId
                        AND otbr.IsActive = 1
                        AND (
                            -- Match by service type and service (most specific)
                            (otbr.ServiceTypeId = stc.ServiceTypeId AND otbr.ServiceId = stc.ServiceId)
                            OR
                            -- Match by service type only (less specific)
                            (otbr.ServiceTypeId = stc.ServiceTypeId AND otbr.ServiceId IS NULL)
                            OR
                            -- Match by service only (less specific)
                            (otbr.ServiceTypeId IS NULL AND otbr.ServiceId = stc.ServiceId)
                            OR
                            -- Match all services (least specific)
                            (otbr.ServiceTypeId IS NULL AND otbr.ServiceId IS NULL)
                        )
                        AND otbr.DayOfWeek = DATEPART(WEEKDAY, stc.[Date]) - 1 -- Convert to 0-6 format (Sunday=0)
                        AND CAST(stc.StartTimeConverted AS TIME) >= otbr.StartTime
                        AND CAST(stc.StartTimeConverted AS TIME) < otbr.EndTime
                        ORDER BY 
                            -- Priority: service type + service > service type only > service only > all services
                            CASE WHEN otbr.ServiceTypeId IS NOT NULL AND otbr.ServiceId IS NOT NULL THEN 1
                                 WHEN otbr.ServiceTypeId IS NOT NULL AND otbr.ServiceId IS NULL THEN 2
                                 WHEN otbr.ServiceTypeId IS NULL AND otbr.ServiceId IS NOT NULL THEN 3
                                 ELSE 4 END
                    ), stc.WageRate) -- Fallback to original wage rate if no time-based rate found
                ELSE stc.WageRate
            END AS FinalWageRate
        FROM ServiceTaskCalculations stc
    ),
    HoursCalculated AS (
        SELECT 
            ServiceTaskId,
            FinalBillingRate AS BillingRate,
            FinalWageRate AS WageRate,
            WageOptionId,
            CASE 
                WHEN StartTimeConverted IS NULL OR EndTimeConverted IS NULL THEN 0
                WHEN EndTimeConverted < StartTimeConverted THEN 
                    DATEDIFF(HOUR, StartTimeConverted, EndTimeConverted + 1)
                ELSE 
                    DATEDIFF(HOUR, StartTimeConverted, EndTimeConverted)
            END AS HoursWorked
        FROM TimeBasedRateCalculations
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

	UPDATE ex
	SET 
	   ex.IsConfirmed = 1
	FROM [dbo].[tblUserExpense] ex
	INNER JOIN @ServiceTaskIds sti ON ex.TaskId = sti.ServiceTaskId
	WHERE ISNULL(ex.IsActive,0)=1

    -- Split visit amount between client / payer per funding rules (Phase 2)
    EXEC [dbo].[uspApplyTaskBillingFunding] @pServicesTasks, @pOrganizationId;

	---- Create recommended indexes for optimal performance
---- Index for tblOrganizationTimeBasedRates lookups
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblOrganizationTimeBasedRates_OrganizationId_DayOfWeek_Time')
--BEGIN
--    CREATE NONCLUSTERED INDEX IX_tblOrganizationTimeBasedRates_OrganizationId_DayOfWeek_Time
--    ON [dbo].[tblOrganizationTimeBasedRates] (OrganizationId, DayOfWeek, StartTime, EndTime)
--    INCLUDE (ServiceTypeId, ServiceId, ClientRate, WageRate)
--    WHERE IsActive = 1;
--    PRINT 'Created index IX_tblOrganizationTimeBasedRates_OrganizationId_DayOfWeek_Time';
--END
--GO

---- Index for tblOrganizationTimeBasedRates service lookups
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblOrganizationTimeBasedRates_ServiceLookup')
--BEGIN
--    CREATE NONCLUSTERED INDEX IX_tblOrganizationTimeBasedRates_ServiceLookup
--    ON [dbo].[tblOrganizationTimeBasedRates] (OrganizationId, ServiceTypeId, ServiceId, DayOfWeek)
--    INCLUDE (StartTime, EndTime, ClientRate, WageRate)
--    WHERE IsActive = 1;
--    PRINT 'Created index IX_tblOrganizationTimeBasedRates_ServiceLookup';
--END
--GO

---- Update existing index to include ServiceTypeId
--IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblServices_Rate')
--BEGIN
--    DROP INDEX IX_tblServices_Rate ON [dbo].[tblServices];
--    PRINT 'Dropped existing IX_tblServices_Rate index';
--END
--GO

--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblServices_Rate_Updated')
--BEGIN
--    CREATE NONCLUSTERED INDEX IX_tblServices_Rate_Updated
--    ON [dbo].[tblServices] (Id) 
--    INCLUDE (Rate, ServiceTypeId)
--    WHERE Rate IS NOT NULL AND Rate > 0;
--    PRINT 'Created updated IX_tblServices_Rate index with ServiceTypeId';
--END
--GO

---- Update existing index to include ServiceRateForBilling
--IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblOrganization_Id')
--BEGIN
--    DROP INDEX IX_tblOrganization_Id ON [dbo].[tblOrganization];
--    PRINT 'Dropped existing IX_tblOrganization_Id index';
--END
--GO

--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblOrganization_Id_Updated')
--BEGIN
--    CREATE NONCLUSTERED INDEX IX_tblOrganization_Id_Updated
--    ON [dbo].[tblOrganization] (Id) 
--    INCLUDE (CalculationTypeId, DefaultBillingRate, DefaultWageRate, ServiceRateForBilling, TimeZone);
--    PRINT 'Created updated IX_tblOrganization_Id index with ServiceRateForBilling';
--END
--GO

--PRINT 'Successfully deployed updated uspCalculateBillingAndWageAmounts stored procedure with Time-Based rates support';
--PRINT 'ServiceRateForBilling values: 1=Default, 2=Service-Specific, 3=Time-Based';

END