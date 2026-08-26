-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Get detailed session conflict information for a user
-- =============================================

CREATE PROCEDURE [dbo].[GetSessionConflictInfo]
    @pUserId UNIQUEIDENTIFIER,
    @pCurrentIPAddress NVARCHAR(45) = NULL,
    @pCurrentBrowserName NVARCHAR(100) = NULL,
    @pCurrentOperatingSystem NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get conflicting sessions (sessions from other devices/browsers)
    SELECT 
        lh.[Id],
        lh.[UserId],
        lh.[UserName],
        lh.[UserEmail],
        lh.[UserType],
        lh.[OrganizationId],
        lh.[FranchiseId],
        lh.[LoginTime],
        lh.[IPAddress],
        lh.[UserAgent],
        lh.[BrowserName],
        lh.[BrowserVersion],
        lh.[OperatingSystem],
        lh.[DeviceType],
        lh.[ScreenResolution],
        lh.[Timezone],
        lh.[Language],
        lh.[Country],
        lh.[City],
        lh.[LoginStatus],
        -- Calculate session duration in minutes
        DATEDIFF(MINUTE, lh.LoginTime, GETDATE()) AS SessionDurationMinutes,
        -- Device identification
        CASE 
            WHEN lh.IPAddress != @pCurrentIPAddress THEN 'Different IP: ' + lh.IPAddress
            WHEN lh.BrowserName != @pCurrentBrowserName THEN 'Different Browser: ' + lh.BrowserName
            WHEN lh.OperatingSystem != @pCurrentOperatingSystem THEN 'Different OS: ' + lh.OperatingSystem
            ELSE 'Unknown'
        END AS ConflictReason,
        -- Location information if available
        CASE 
            WHEN lh.Country IS NOT NULL AND lh.City IS NOT NULL 
            THEN lh.City + ', ' + lh.Country
            WHEN lh.Country IS NOT NULL 
            THEN lh.Country
            WHEN lh.IPAddress IS NOT NULL 
            THEN 'IP: ' + lh.IPAddress
            ELSE 'Unknown Location'
        END AS LocationInfo
    FROM [dbo].[tblLoginHistory] lh
    WHERE lh.UserId = @pUserId 
      AND lh.LogoutTime IS NULL 
      AND lh.IsActive = 1
      AND lh.LoginStatus = 'Success'
      AND (
          lh.IPAddress != @pCurrentIPAddress 
          OR lh.BrowserName != @pCurrentBrowserName 
          OR lh.OperatingSystem != @pCurrentOperatingSystem
      )
    ORDER BY lh.LoginTime DESC;
    
    -- Get summary statistics
    SELECT 
        COUNT(*) AS TotalConflictingSessions,
        COUNT(DISTINCT IPAddress) AS UniqueIPAddresses,
        COUNT(DISTINCT BrowserName) AS UniqueBrowsers,
        COUNT(DISTINCT OperatingSystem) AS UniqueOperatingSystems,
        MAX(LoginTime) AS MostRecentConflictingLogin,
        MIN(LoginTime) AS OldestConflictingLogin,
        AVG(CAST(DATEDIFF(MINUTE, LoginTime, GETDATE()) AS FLOAT)) AS AverageSessionDurationMinutes
    FROM [dbo].[tblLoginHistory]
    WHERE UserId = @pUserId 
      AND LogoutTime IS NULL 
      AND IsActive = 1
      AND LoginStatus = 'Success'
      AND (
          IPAddress != @pCurrentIPAddress 
          OR BrowserName != @pCurrentBrowserName 
          OR OperatingSystem != @pCurrentOperatingSystem
      );
END