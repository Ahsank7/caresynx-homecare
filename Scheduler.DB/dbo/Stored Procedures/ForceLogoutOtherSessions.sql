-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Force logout from other active sessions for a user
-- =============================================

CREATE PROCEDURE [dbo].[ForceLogoutOtherSessions]
    @pUserId UNIQUEIDENTIFIER,
    @pCurrentIPAddress NVARCHAR(45) = NULL,
    @pCurrentBrowserName NVARCHAR(100) = NULL,
    @pCurrentOperatingSystem NVARCHAR(100) = NULL,
    @pReason NVARCHAR(500) = 'Logged in from another device'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME = GETDATE();
    DECLARE @RowsAffected INT = 0;
    
    -- Update all other active sessions to mark them as logged out
    UPDATE [dbo].[tblLoginHistory]
    SET 
        LogoutTime = @CurrentTime,
        SessionDuration = DATEDIFF(MINUTE, LoginTime, @CurrentTime),
        ModifiedDate = @CurrentTime,
        LoginStatus = 'ForceLoggedOut',
        FailureReason = @pReason
    WHERE UserId = @pUserId 
      AND LogoutTime IS NULL 
      AND IsActive = 1
      AND LoginStatus = 'Success'
      AND (
          IPAddress != @pCurrentIPAddress 
          OR BrowserName != @pCurrentBrowserName 
          OR OperatingSystem != @pCurrentOperatingSystem
      );
    
    SET @RowsAffected = @@ROWCOUNT;
    
    -- Return the number of sessions that were force logged out
    SELECT @RowsAffected AS SessionsForceLoggedOut;
END