-- =============================================    
-- Author:  <Author,,Name>    
-- Create date: <Create Date,,>    
-- Description: Get Service Providers with Availability Status, Leave Status, and Profile Images
-- Updated: Added timezone support and multi-day availability checking
-- =============================================    
    
CREATE PROCEDURE [ServiceProvider].[uspGetServiceProvidersWithAvailability]      
-- Add the parameters for the stored procedure here    
 @pFranchiseId uniqueidentifier,     
 @pStartDate date = null,  
 @pEndDate date = null,
 @pStartTime time = null,
 @pEndTime time = null,
 @pSearchText nvarchar(100) = null
AS    
BEGIN    
 -- SET NOCOUNT ON added to prevent extra result sets from    
 -- interfering with SELECT statements.    
 SET NOCOUNT ON;    
 
  IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults    
  IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results    
  IF OBJECT_ID('tempdb..#AvailabilityCheck') IS NOT NULL DROP TABLE #AvailabilityCheck    
  IF OBJECT_ID('tempdb..#LeaveCheck') IS NOT NULL DROP TABLE #LeaveCheck    
  IF OBJECT_ID('tempdb..#TaskCheck') IS NOT NULL DROP TABLE #TaskCheck
  IF OBJECT_ID('tempdb..#DateRange') IS NOT NULL DROP TABLE #DateRange    
    
  -- Set default values if null
  if isnull(@pStartDate,'') = '' set @pStartDate = GETDATE()
  if isnull(@pEndDate,'') = '' set @pEndDate = GETDATE()
  if isnull(@pStartTime,'') = '' set @pStartTime = '00:00:00'
  if isnull(@pEndTime,'') = '' set @pEndTime = '23:59:59'
  if isnull(@pSearchText,'') = '' set @pSearchText = null  

  -- Get Organization ID from Franchise
  DECLARE @pOrganizationId UNIQUEIDENTIFIER
  SELECT @pOrganizationId = OrganizationId 
  FROM [dbo].[tblFranchise] 
  WHERE Id = @pFranchiseId AND IsActive = 1

  -- Get timezone from organization settings
  DECLARE @pTimeZone NVARCHAR(50)
  SELECT @pTimeZone = [TimeZone] 
  FROM [dbo].[tblOrganization] 
  WHERE [Id] = @pOrganizationId AND [IsActive] = 1
  
  -- If no organization timezone found, use default
  IF ISNULL(@pTimeZone,'') = ''
    SET @pTimeZone = 'Pakistan Standard Time'
  
  -- Convert timezone name to SQL Server timezone identifier
  DECLARE @pTimeZoneId NVARCHAR(50)
  SET @pTimeZoneId = [dbo].[GetTimeZoneId](@pTimeZone)

  -- Create table with all dates in the range
  CREATE TABLE #DateRange (
    DateValue DATE,
    DayOfWeek NVARCHAR(12)
  )

  -- Populate date range table
  DECLARE @CurrentDate DATE = @pStartDate
  WHILE @CurrentDate <= @pEndDate
  BEGIN
    INSERT INTO #DateRange (DateValue, DayOfWeek)
    VALUES (@CurrentDate, DATENAME(WEEKDAY, @CurrentDate))
    
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate)
  END
  
  -- Get base service provider list
  SELECT     
    U.[Id] as UserId,   
    (ISNULL(U.UserNo,'') + ': ' + U.[FirstName] + ' ' + U.[LastName]) as [Name],
    U.[FirstName],
    U.[LastName],
    U.[UserNo],
    U.[ProfileImagePath],
    U.[Email],
    U.[MobileNo],
    U.[PhoneNo]
  INTO #Results        
  FROM [dbo].[tblUser] U    
  JOIN [dbo].tblLookupItems LI on LookupType='UserStatus' and LI.Id=U.[Status]    
  WHERE 1=1     
    and U.UserType=2  
    and U.FranchiseId=@pFranchiseId
    and U.IsActive = 1
    AND (
        @pSearchText IS NULL OR
        U.UserNo LIKE '%' + @pSearchText + '%' OR
        U.FirstName LIKE '%' + @pSearchText + '%' OR
        U.LastName LIKE '%' + @pSearchText + '%' OR
        U.Email LIKE '%' + @pSearchText + '%'
    );

  -- Check availability for each provider across ALL days in the range
  -- Using simple JOIN approach for better performance
  
  -- Count total days in the range
  DECLARE @TotalDaysInRange INT
  SELECT @TotalDaysInRange = COUNT(*) FROM #DateRange
  
  -- Pre-calculate total availability records per provider for efficiency
  SELECT 
    UserId,
    COUNT(*) as TotalRecords
  INTO #UserAvailabilityCounts
  FROM [dbo].[tblUserAvailability]
  WHERE IsActive = 1
  GROUP BY UserId
  
  -- Join providers with date range and availability, count matching days
  SELECT 
    R.UserId,
    -- Count how many days in the range have matching availability
    COUNT(DISTINCT CASE 
      WHEN UA.Id IS NOT NULL THEN DR.DateValue 
      ELSE NULL 
    END) as AvailableDaysCount,
    -- Get total availability records for this provider
    ISNULL(UAC.TotalRecords, 0) as TotalAvailabilityRecords,
    -- Get sample availability times (from first matching day) - convert to VARCHAR for C# compatibility
    CONVERT(VARCHAR(8), MIN(UA.StartTime), 108) as AvailableStartTime,
    CONVERT(VARCHAR(8), MAX(UA.EndTime), 108) as AvailableEndTime,
    MIN(UA.Day) as AvailableDay
  INTO #AvailabilityCheckTemp
  FROM #Results R
  CROSS JOIN #DateRange DR
  LEFT JOIN [dbo].[tblUserAvailability] UA 
    ON R.UserId = UA.UserId 
    AND UA.Day = DR.DayOfWeek 
    AND UA.IsActive = 1
    AND @pStartTime >= UA.StartTime 
    AND @pEndTime <= UA.EndTime
  LEFT JOIN #UserAvailabilityCounts UAC ON R.UserId = UAC.UserId
  GROUP BY R.UserId, UAC.TotalRecords
  
  -- Add availability status based on counts
  SELECT 
    UserId,
    AvailableDaysCount,
    TotalAvailabilityRecords,
    AvailableStartTime,
    AvailableEndTime,
    AvailableDay,
    CASE
      -- If available for ALL days in the range
      WHEN AvailableDaysCount = @TotalDaysInRange THEN 'Available'
      -- If has some availability records but not covering all days
      WHEN TotalAvailabilityRecords > 0 THEN 'Not Available'
      -- No availability records at all
      ELSE 'No Availability Set'
    END as AvailabilityStatus
  INTO #AvailabilityCheck
  FROM #AvailabilityCheckTemp
  
  -- Cleanup
  DROP TABLE #UserAvailabilityCounts
  DROP TABLE #AvailabilityCheckTemp

  -- Check leave status for each provider
  -- Check if provider has any approved leave that overlaps with the date range
  SELECT 
    R.UserId,
    CASE 
      WHEN UL.Id IS NOT NULL THEN 'On Leave'
      ELSE 'No Leave'
    END as LeaveStatus,
    CONVERT(VARCHAR(10), UL.StartDate, 23) as LeaveStartDate,
    CONVERT(VARCHAR(10), UL.EndDate, 23) as LeaveEndDate
  INTO #LeaveCheck
  FROM #Results R
  LEFT JOIN [dbo].[tblUserLeave] UL ON R.UserId = UL.UserId 
    AND UL.Status = 1 -- Approved leave
    AND (
      -- Leave overlaps with requested date range
      (UL.StartDate <= @pEndDate AND UL.EndDate >= @pStartDate)
    );

  -- Check existing tasks for each provider with timezone conversion
  -- Convert UTC task times to organization timezone for proper comparison
  SELECT 
    R.UserId,
    CASE 
      WHEN COUNT(ST.Id) > 0 THEN 'Busy'
      ELSE 'No Tasks'
    END as TaskStatus,
    COUNT(ST.Id) as TaskCount
  INTO #TaskCheck
  FROM #Results R
  LEFT JOIN [dbo].[tblServicesTask] ST ON R.UserId = ST.ServiceProviderId 
    AND ST.Status IN (1,2,3) -- Scheduled, In-Progress, Completed
    AND ST.Date BETWEEN @pStartDate AND @pEndDate
    AND (
      -- Check if task time overlaps with requested time (after timezone conversion)
      (
        -- Convert UTC times to organization timezone
        CAST(CAST(ST.[StartTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) AS TIME) < @pEndTime
        AND
        CAST(CAST(ST.[EndTime] AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) AS TIME) > @pStartTime
      )
    )
  GROUP BY R.UserId;

  -- Combine all results
  SELECT 
    R.UserId,
    R.[Name],
    R.[FirstName],
    R.[LastName],
    R.[UserNo],
    R.[ProfileImagePath],
    R.[Email],
    R.[MobileNo],
    R.[PhoneNo],
    -- Determine final availability status with priority: Leave > Busy > Availability
    CASE 
      WHEN LC.LeaveStatus = 'On Leave' THEN 'On Leave'
      WHEN TC.TaskStatus = 'Busy' THEN 'Busy'
      WHEN AC.AvailabilityStatus = 'Available' THEN 'Available'
      WHEN AC.AvailabilityStatus = 'Not Available' THEN 'Not Available'
      ELSE 'No Availability Set'
    END as FinalAvailabilityStatus,
    AC.AvailabilityStatus as RawAvailabilityStatus,
    LC.LeaveStatus as LeaveStatus,
    TC.TaskStatus as TaskStatus,
    AC.AvailableStartTime,
    AC.AvailableEndTime,
    AC.AvailableDay,
    LC.LeaveStartDate,
    LC.LeaveEndDate,
    TC.TaskCount
  INTO #FinalResults
  FROM #Results R
  LEFT JOIN #AvailabilityCheck AC ON R.UserId = AC.UserId
  LEFT JOIN #LeaveCheck LC ON R.UserId = LC.UserId
  LEFT JOIN #TaskCheck TC ON R.UserId = TC.UserId;

  -- Return final results
  SELECT * FROM #FinalResults
  ORDER BY [Name];
  
  SELECT COUNT(*) TotalRecords FROM #FinalResults;

  -- Cleanup temp tables
  IF OBJECT_ID('tempdb..#DateRange') IS NOT NULL DROP TABLE #DateRange
  IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results
  IF OBJECT_ID('tempdb..#AvailabilityCheck') IS NOT NULL DROP TABLE #AvailabilityCheck
  IF OBJECT_ID('tempdb..#LeaveCheck') IS NOT NULL DROP TABLE #LeaveCheck
  IF OBJECT_ID('tempdb..#TaskCheck') IS NOT NULL DROP TABLE #TaskCheck
  IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults
  IF OBJECT_ID('tempdb..#UserAvailabilityCounts') IS NOT NULL DROP TABLE #UserAvailabilityCounts
  IF OBJECT_ID('tempdb..#AvailabilityCheckTemp') IS NOT NULL DROP TABLE #AvailabilityCheckTemp

END
