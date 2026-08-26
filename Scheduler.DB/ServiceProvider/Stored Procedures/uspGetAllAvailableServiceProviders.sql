-- =============================================    
-- Author:  <Author,,Name>    
-- Create date: <Create Date,,>    
-- Description: <Description,,>    
-- =============================================    
    
    
CREATE PROCEDURE [ServiceProvider].[uspGetAllAvailableServiceProviders]      
-- Add the parameters for the stored procedure here    
 @pFranchiseId uniqueidentifier,     
 @pStartDateTime datetime=null,  
 @pEndDateTime datetime=null,
 @pSearchText nvarchar(100)=null,
 @pOrganizationId UNIQUEIDENTIFIER=null,
 @PageNumber int = 1,
 @pPageSize int = 10
AS    
BEGIN    
 -- SET NOCOUNT ON added to prevent extra result sets from    
 -- interfering with SELECT statements.    
 SET NOCOUNT ON;    
    
  IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults    
  IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results    
  IF OBJECT_ID('tempdb..#AvailableUsers') IS NOT NULL DROP TABLE #AvailableUsers
  IF OBJECT_ID('tempdb..#UnavailableUsers') IS NOT NULL DROP TABLE #UnavailableUsers
    
    
   if isnull(@pEndDateTime,'') =''    
   set @pEndDateTime=null    
   
   -- Get timezone from organization settings
   DECLARE @pTimeZone NVARCHAR(50)
   SELECT @pTimeZone = [TimeZone] FROM [dbo].[tblOrganization] WHERE [Id] = @pOrganizationId AND [IsActive] = 1
   
   -- If no organization timezone found, use default
   If IsNull(@pTimeZone,'') = ''
     Set @pTimeZone='Pakistan Standard Time'
   
   -- Convert timezone name to SQL Server timezone identifier
   DECLARE @pTimeZoneId NVARCHAR(50)
   SET @pTimeZoneId = [dbo].[GetTimeZoneId](@pTimeZone)
    
   if isnull(@pStartDateTime,'') =''    
   set @pStartDateTime=null    
    
   if isnull(@pSearchText,'') =''    
   set @pSearchText=null  
    
   -- Get all service providers for the franchise
   SELECT     
    U.[Id] as UserId ,   
    ( ISNULL(U.UserNo,'')+': '+   U.[FirstName] +' '+U.[LastName] ) as [Name],
    U.[FirstName],
    U.[LastName],
    U.[UserNo],
    U.[Email]
   INTO #AvailableUsers        
   From  [dbo].[tblUser] U    
   JOIN [dbo].tblLookupItems LI on LookupType='UserStatus' and  LI.Id=U.[Status]    
   WHERE 1=1     
   and U.UserType=2  
   and U.FranchiseId=@pFranchiseId
   AND (
        @pSearchText IS NULL OR
        U.UserNo LIKE '%' + @pSearchText + '%' OR
        U.FirstName LIKE '%' + @pSearchText + '%' OR
        U.LastName LIKE '%' + @pSearchText + '%' OR
        U.Email LIKE '%' + @pSearchText + '%'
    );

   -- If no date range provided, return all service providers with pagination
   IF @pStartDateTime IS NULL OR @pEndDateTime IS NULL
   BEGIN
       SELECT * 
       FROM #AvailableUsers
       ORDER BY [Name]
       OFFSET (@PageNumber-1)*@pPageSize ROWS
       FETCH NEXT @pPageSize ROWS ONLY
       
       SELECT COUNT(*) TotalRecords FROM #AvailableUsers
       RETURN
   END

   -- Find users who are unavailable due to leave records
   SELECT DISTINCT U.UserId
   INTO #UnavailableUsers
   FROM #AvailableUsers U
   WHERE EXISTS (
       SELECT 1 FROM [dbo].[tblUserLeave] UL
       WHERE UL.UserId = U.UserId 
       AND ISNULL(UL.IsActive, 0) = 1
       AND (
           -- Check if leave period overlaps with requested time range (with timezone conversion)
           (CAST(UL.StartTime AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) <= @pEndDateTime 
            AND CAST(UL.EndTime AT TIME ZONE 'UTC' AT TIME ZONE @pTimeZoneId AS DATETIME) >= @pStartDateTime)
           OR 
           -- Check if leave date falls within the requested date range
           (UL.Date BETWEEN CAST(@pStartDateTime AS DATE) AND CAST(@pEndDateTime AS DATE))
       )
   )

   -- Find users who have availability restrictions
   INSERT INTO #UnavailableUsers
   SELECT DISTINCT U.UserId
   FROM #AvailableUsers U
   WHERE EXISTS (
       SELECT 1 FROM [dbo].[tblUserAvailability] UA
       WHERE UA.UserId = U.UserId 
       AND ISNULL(UA.IsActive, 0) = 1
       AND UA.Day = DATENAME(WEEKDAY, @pStartDateTime)
       AND (
           -- Check if the requested time falls outside their available hours
           CAST(@pStartDateTime AS TIME) < UA.StartTime 
           OR CAST(@pEndDateTime AS TIME) > UA.EndTime
       )
   )

   -- Get available users (those not in unavailable list)
   SELECT 
       UserId,
       [Name],
       [FirstName],
       [LastName],
       [UserNo],
       [Email],
       'Available' as AvailabilityStatus
   INTO #Results
   FROM #AvailableUsers
   WHERE UserId NOT IN (SELECT UserId FROM #UnavailableUsers)

   -- If no users are available, check if any users have no restrictions at all
   IF NOT EXISTS (SELECT 1 FROM #Results)
   BEGIN
       -- Users with no availability or leave records are considered available
       INSERT INTO #Results
       SELECT 
           U.UserId,
           U.[Name],
           U.[FirstName],
           U.[LastName],
           U.[UserNo],
           U.[Email],
           'Available (No Restrictions)' as AvailabilityStatus
       FROM #AvailableUsers U
       WHERE U.UserId NOT IN (
           SELECT DISTINCT UserId FROM [dbo].[tblUserAvailability] WHERE ISNULL(IsActive, 0) = 1
           UNION
           SELECT DISTINCT UserId FROM [dbo].[tblUserLeave] WHERE ISNULL(IsActive, 0) = 1
       )
   END

   -- Copy to final results with pagination
   SELECT * 
   INTO #FinalResults 
   FROM #Results
   ORDER BY [Name]
   OFFSET (@PageNumber-1)*@pPageSize ROWS
   FETCH NEXT @pPageSize ROWS ONLY

   SELECT * FROM #FinalResults
   SELECT COUNT(*) TotalRecords FROM #Results

END