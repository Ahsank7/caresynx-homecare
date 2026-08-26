CREATE PROCEDURE [dbo].[SaveUpdateMenu]
    @pId UNIQUEIDENTIFIER = NULL,
    @pMenuId NVARCHAR(100),
    @pMenuName NVARCHAR(200),
    @pParentMenuId NVARCHAR(100) = NULL,
    @pMenuPath NVARCHAR(500) = NULL,
    @pMenuIcon NVARCHAR(100) = NULL,
    @pMenuOrder INT = 0,
    @pIsActive BIT = 1,
    @pCreatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @pId IS NULL OR NOT EXISTS (SELECT 1 FROM tblMenu WHERE Id = @pId)
    BEGIN
        -- Insert new menu
        INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, IsActive, CreatedBy)
        VALUES (NEWID(), @pMenuId, @pMenuName, @pParentMenuId, @pMenuPath, @pMenuIcon, @pMenuOrder, @pIsActive, @pCreatedBy);
    END
    ELSE
    BEGIN
        -- Update existing menu
        UPDATE tblMenu
        SET MenuId = @pMenuId,
            MenuName = @pMenuName,
            ParentMenuId = @pParentMenuId,
            MenuPath = @pMenuPath,
            MenuIcon = @pMenuIcon,
            MenuOrder = @pMenuOrder,
            IsActive = @pIsActive
        WHERE Id = @pId;
    END

    SELECT 1 AS Success;
END