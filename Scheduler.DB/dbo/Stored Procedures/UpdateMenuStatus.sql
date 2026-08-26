CREATE PROCEDURE [dbo].[UpdateMenuStatus]
    @pMenuId UNIQUEIDENTIFIER,
    @pIsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @vMenuKey NVARCHAR(100);
    DECLARE @vOrganizationId UNIQUEIDENTIFIER;

    SELECT
        @vMenuKey = MenuId,
        @vOrganizationId = OrganizationId
    FROM tblMenu
    WHERE Id = @pMenuId;

    IF @vMenuKey IS NULL
    BEGIN
        SELECT 0 AS AffectedRows;
        RETURN;
    END

    DECLARE @vAffectedRows INT = 0;

    UPDATE tblMenu
    SET IsActive = @pIsActive
    WHERE Id = @pMenuId;

    SET @vAffectedRows = @@ROWCOUNT;

    -- When disabling a menu from admin, also revoke all role permissions for that menu.
    -- Keeping entries with all flags = 0 ensures it remains denied if menu is re-enabled later.
    IF @pIsActive = 0
    BEGIN
        UPDATE rp
        SET
            rp.CanView = 0,
            rp.CanCreate = 0,
            rp.CanEdit = 0,
            rp.CanDelete = 0,
            rp.IsActive = 1
        FROM tblRolePermission rp
        WHERE rp.MenuId = @vMenuKey
          AND (
              rp.OrganizationId = @vOrganizationId
              OR (@vOrganizationId IS NULL AND rp.OrganizationId IS NULL)
          );
    END

    SELECT @vAffectedRows AS AffectedRows;
END