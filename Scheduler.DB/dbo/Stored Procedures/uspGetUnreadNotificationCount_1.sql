CREATE   PROCEDURE [dbo].[uspGetUnreadNotificationCount]
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER = NULL,
    @FranchiseId UNIQUEIDENTIFIER = NULL,
    @RoleId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(DISTINCT n.Id) AS UnreadCount
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
END