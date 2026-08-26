CREATE   PROCEDURE [dbo].[uspMarkNotificationAsRead]
    @NotificationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [dbo].[tblNotificationRead] WHERE NotificationId = @NotificationId AND UserId = @UserId)
        BEGIN
            INSERT INTO [dbo].[tblNotificationRead] (Id, NotificationId, UserId, ReadDate, IsRead)
            VALUES (NEWID(), @NotificationId, @UserId, GETDATE(), 1);
        END
        ELSE
        BEGIN
            UPDATE [dbo].[tblNotificationRead]
            SET ReadDate = GETDATE(), IsRead = 1
            WHERE NotificationId = @NotificationId AND UserId = @UserId;
        END
        
        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END