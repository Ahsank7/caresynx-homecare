CREATE OR ALTER PROCEDURE [dbo].[uspDeleteNotification]
    @NotificationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Soft delete by setting IsActive to 0
        -- Only the creator or an admin can delete
        UPDATE [dbo].[tblNotification]
        SET IsActive = 0
        WHERE Id = @NotificationId
            AND (CreatedBy = @UserId OR EXISTS (
                SELECT 1 FROM [dbo].[tblUser] u
                INNER JOIN [dbo].[tblRole] r ON u.RoleId = r.Id
                WHERE u.Id = @UserId AND r.RoleLevel <= 2 -- Admin or Super Admin
            ));
        
        IF @@ROWCOUNT > 0
            SELECT 1 AS Success;
        ELSE
            SELECT 0 AS Success, 'Notification not found or you do not have permission to delete it.' AS ErrorMessage;
            
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END
GO

