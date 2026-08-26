CREATE   PROCEDURE [dbo].[GetUserMenuPermissions]
    @pUserId uniqueidentifier,
    @pOrganizationId uniqueidentifier = null
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vRoleId int, @vFranchiseId uniqueidentifier

    -- Get user role and franchise
    SELECT @vRoleId = ur.RoleId,
           @vFranchiseId = u.FranchiseId
    FROM tblUser u
    LEFT JOIN tblUserRole ur ON ur.UserId = u.Id AND ur.IsActive = 1
    WHERE u.Id = @pUserId

    -- If no organization provided, get it from franchise
    IF @pOrganizationId IS NULL
    BEGIN
        SELECT @pOrganizationId = f.OrganizationId
        FROM tblFranchise f
        WHERE f.Id = @vFranchiseId
    END

    -- Include inactive menus so clients get explicit CanView=0 (omitting a menu makes the UI treat it as allowed).
    -- When admin disables a menu (IsActive=0), all effective permissions are denied regardless of tblRolePermission.
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
        AND rp.RoleId = @vRoleId 
        AND (rp.OrganizationId = @pOrganizationId OR rp.OrganizationId IS NULL)
        AND rp.IsActive = 1
    ORDER BY m.MenuOrder, m.MenuName
END