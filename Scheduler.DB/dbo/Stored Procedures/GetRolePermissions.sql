CREATE   PROCEDURE [dbo].[GetRolePermissions]
    @pRoleId int,
    @pOrganizationId uniqueidentifier = null
AS
BEGIN
    SET NOCOUNT ON;

    -- Include inactive menus with effective deny so Access Control matches runtime permissions.
    SELECT 
        m.MenuId,
        m.MenuName,
        m.ParentMenuId,
        m.MenuPath,
        m.MenuIcon,
        m.MenuOrder,
        CASE WHEN m.IsActive = 0 THEN CAST(0 AS BIT) ELSE CAST(ISNULL(rp.CanView, 1) AS BIT) END AS CanView,
        CASE WHEN m.IsActive = 0 THEN CAST(0 AS BIT) ELSE CAST(ISNULL(rp.CanCreate, 0) AS BIT) END AS CanCreate,
        CASE WHEN m.IsActive = 0 THEN CAST(0 AS BIT) ELSE CAST(ISNULL(rp.CanEdit, 0) AS BIT) END AS CanEdit,
        CASE WHEN m.IsActive = 0 THEN CAST(0 AS BIT) ELSE CAST(ISNULL(rp.CanDelete, 0) AS BIT) END AS CanDelete
    FROM tblMenu m
    LEFT JOIN tblRolePermission rp ON rp.MenuId = m.MenuId 
        AND rp.RoleId = @pRoleId 
        AND (rp.OrganizationId = @pOrganizationId OR rp.OrganizationId IS NULL)
        AND rp.IsActive = 1
    ORDER BY m.MenuOrder, m.MenuName
END
