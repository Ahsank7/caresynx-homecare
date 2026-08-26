-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Check for active user sessions and return conflicting sessions
-- =============================================

CREATE PROCEDURE [dbo].[CheckActiveUserSessions]
    @pUserId UNIQUEIDENTIFIER,
    @pCurrentIPAddress NVARCHAR(45) = NULL,
    @pCurrentUserAgent NVARCHAR(500) = NULL,
    @pCurrentBrowserName NVARCHAR(100) = NULL,
    @pCurrentOperatingSystem NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all active sessions for the user (where LogoutTime is NULL)
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
        -- Check if this is the current session (same IP, browser, OS)
        CASE 
            WHEN lh.IPAddress = @pCurrentIPAddress 
                AND lh.BrowserName = @pCurrentBrowserName 
                AND lh.OperatingSystem = @pCurrentOperatingSystem
            THEN 1 
            ELSE 0 
        END AS IsCurrentSession,
        -- Check if this is a different device/browser
        CASE 
            WHEN lh.IPAddress != @pCurrentIPAddress 
                OR lh.BrowserName != @pCurrentBrowserName 
                OR lh.OperatingSystem != @pCurrentOperatingSystem
            THEN 1 
            ELSE 0 
        END AS IsDifferentDevice
    FROM [dbo].[tblLoginHistory] lh
    WHERE lh.UserId = @pUserId 
      AND lh.LogoutTime IS NULL 
      AND lh.IsActive = 1
      AND lh.LoginStatus = 'Success'
    ORDER BY lh.LoginTime DESC;
    
    -- Return summary information
    SELECT 
        COUNT(*) AS TotalActiveSessions,
        SUM(CASE 
            WHEN IPAddress != @pCurrentIPAddress 
                OR BrowserName != @pCurrentBrowserName 
                OR OperatingSystem != @pCurrentOperatingSystem
            THEN 1 
            ELSE 0 
        END) AS SessionsFromOtherDevices,
        MAX(LoginTime) AS MostRecentLoginTime,
        MIN(LoginTime) AS OldestActiveLoginTime
    FROM [dbo].[tblLoginHistory]
    WHERE UserId = @pUserId 
      AND LogoutTime IS NULL 
      AND IsActive = 1
      AND LoginStatus = 'Success';
END