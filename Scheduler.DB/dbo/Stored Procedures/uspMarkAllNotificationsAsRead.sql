CREATE OR ALTER PROCEDURE [dbo].[uspMarkAllNotificationsAsRead]
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER = NULL,
    @FranchiseId UNIQUEIDENTIFIER = NULL,
    @RoleId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Get all unread notifications for the user
        INSERT INTO [dbo].[tblNotificationRead] (Id, NotificationId, UserId, ReadDate, IsRead)
        SELECT 
            NEWID(),
            n.Id,
            @UserId,
            GETDATE(),
            1
        FROM [dbo].[tblNotification] n
        LEFT JOIN [dbo].[tblNotificationRead] nr ON n.Id = nr.NotificationId AND nr.UserId = @UserId
        WHERE n.IsActive = 1
            AND (n.ExpiresAt IS NULL OR n.ExpiresAt > GETDATE())
            AND nr.Id IS NULL
            AND (
                n.TargetUserId = @UserId
                OR (n.TargetUserId IS NULL AND (n.OrganizationId = @OrganizationId OR n.OrganizationId IS NULL))
            )
            AND (n.FranchiseId IS NULL OR n.FranchiseId = @FranchiseId)
            AND (n.TargetRoleId IS NULL OR n.TargetRoleId = @RoleId);
        
        COMMIT TRANSACTION;
        
        SELECT @@ROWCOUNT AS MarkedCount;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

