CREATE OR ALTER PROCEDURE [dbo].[uspGetUserNotifications]
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER = NULL,
    @FranchiseId UNIQUEIDENTIFIER = NULL,
    @RoleId INT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20,
    @UnreadOnly BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    DECLARE @TotalRecords INT;
    DECLARE @UnreadCount INT;
    
    SELECT @TotalRecords = COUNT(DISTINCT n.Id)
    FROM [dbo].[tblNotification] n
    LEFT JOIN [dbo].[tblNotificationRead] nr ON n.Id = nr.NotificationId AND nr.UserId = @UserId
    WHERE n.IsActive = 1
        AND (n.ExpiresAt IS NULL OR n.ExpiresAt > GETDATE())
        AND (
            -- User-specific notification
            n.TargetUserId = @UserId
            OR 
            -- Organization-wide notification
            (n.TargetUserId IS NULL AND (n.OrganizationId = @OrganizationId OR n.OrganizationId IS NULL))
        )
        AND (
            -- Franchise-specific
            n.FranchiseId IS NULL 
            OR n.FranchiseId = @FranchiseId
        )
        AND (
            -- Role-specific
            n.TargetRoleId IS NULL 
            OR n.TargetRoleId = @RoleId
        )
        AND (
            @UnreadOnly = 0 OR nr.Id IS NULL
        );
    
    -- Get unread count
    SELECT @UnreadCount = COUNT(DISTINCT n.Id)
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
    
    -- Get paginated notifications
    SELECT 
        n.Id,
        n.Title,
        n.Message,
        n.Type,
        n.Priority,
        n.OrganizationId,
        n.FranchiseId,
        n.TargetRoleId,
        n.TargetUserId,
        n.ActivityType,
        n.ActivityEntity,
        n.ActivityEntityId,
        n.ActionUrl,
        n.CreatedBy,
        n.CreatedDate,
        n.ExpiresAt,
        CASE WHEN nr.Id IS NULL THEN 0 ELSE 1 END AS IsRead,
        nr.ReadDate,
        creator.FirstName + ' ' + creator.LastName AS CreatedByName,
        @TotalRecords AS TotalRecords,
        @UnreadCount AS UnreadCount
    FROM [dbo].[tblNotification] n
    LEFT JOIN [dbo].[tblNotificationRead] nr ON n.Id = nr.NotificationId AND nr.UserId = @UserId
    LEFT JOIN [dbo].[tblUser] creator ON n.CreatedBy = creator.Id
    WHERE n.IsActive = 1
        AND (n.ExpiresAt IS NULL OR n.ExpiresAt > GETDATE())
        AND (
            n.TargetUserId = @UserId
            OR (n.TargetUserId IS NULL AND (n.OrganizationId = @OrganizationId OR n.OrganizationId IS NULL))
        )
        AND (n.FranchiseId IS NULL OR n.FranchiseId = @FranchiseId)
        AND (n.TargetRoleId IS NULL OR n.TargetRoleId = @RoleId)
        AND (@UnreadOnly = 0 OR nr.Id IS NULL)
    ORDER BY 
        n.Priority DESC,
        n.CreatedDate DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END
GO

