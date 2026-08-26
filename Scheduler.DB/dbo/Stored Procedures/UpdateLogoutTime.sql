-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	Update logout time and calculate session duration for a login session
-- =============================================

CREATE PROCEDURE [dbo].[UpdateLogoutTime]
    @pUserId UNIQUEIDENTIFIER,
    @pLoginTime DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentTime DATETIME = GETDATE();
    DECLARE @SessionDuration INT;
    
    -- If no specific login time provided, use the most recent active session
    IF @pLoginTime IS NULL
    BEGIN
        SELECT TOP 1 @pLoginTime = LoginTime
        FROM [dbo].[tblLoginHistory]
        WHERE UserId = @pUserId 
          AND LogoutTime IS NULL 
          AND IsActive = 1
        ORDER BY LoginTime DESC;
    END
    
    -- Calculate session duration in minutes
    SET @SessionDuration = DATEDIFF(MINUTE, @pLoginTime, @CurrentTime);
    
    -- Update the login history record
    UPDATE [dbo].[tblLoginHistory]
    SET 
        LogoutTime = @CurrentTime,
        SessionDuration = @SessionDuration,
        ModifiedDate = @CurrentTime
    WHERE UserId = @pUserId 
      AND LoginTime = @pLoginTime
      AND LogoutTime IS NULL
      AND IsActive = 1;
    
    -- Return the number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END