CREATE OR ALTER PROCEDURE [dbo].[uspCreateNotification]
    @Title NVARCHAR(200),
    @Message NVARCHAR(MAX),
    @Type NVARCHAR(50) = 'Info',
    @Priority INT = 0,
    @OrganizationId UNIQUEIDENTIFIER = NULL,
    @FranchiseId UNIQUEIDENTIFIER = NULL,
    @TargetRoleId INT = NULL,
    @TargetUserId UNIQUEIDENTIFIER = NULL,
    @ActivityType NVARCHAR(100) = NULL,
    @ActivityEntity NVARCHAR(100) = NULL,
    @ActivityEntityId UNIQUEIDENTIFIER = NULL,
    @ActionUrl NVARCHAR(500) = NULL,
    @CreatedBy UNIQUEIDENTIFIER,
    @ExpiresAt DATETIME = NULL,
    @NotificationId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        SET @NotificationId = NEWID();
        
        INSERT INTO [dbo].[tblNotification] (
            Id,
            Title,
            Message,
            Type,
            Priority,
            OrganizationId,
            FranchiseId,
            TargetRoleId,
            TargetUserId,
            ActivityType,
            ActivityEntity,
            ActivityEntityId,
            ActionUrl,
            CreatedBy,
            CreatedDate,
            IsActive,
            ExpiresAt
        )
        VALUES (
            @NotificationId,
            @Title,
            @Message,
            @Type,
            @Priority,
            @OrganizationId,
            @FranchiseId,
            @TargetRoleId,
            @TargetUserId,
            @ActivityType,
            @ActivityEntity,
            @ActivityEntityId,
            @ActionUrl,
            @CreatedBy,
            GETDATE(),
            1,
            @ExpiresAt
        );
        
        COMMIT TRANSACTION;
        
        SELECT @NotificationId AS Id;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
GO

