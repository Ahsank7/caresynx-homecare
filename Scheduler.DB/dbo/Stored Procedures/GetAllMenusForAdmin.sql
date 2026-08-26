CREATE PROCEDURE [dbo].[GetAllMenusForAdmin]
  @pOrganizationId uniqueIdentifier
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MenuId,
        MenuName,
        ParentMenuId,
        MenuPath,
        MenuIcon,
        MenuOrder,
        IsActive
    FROM tblMenu
    WHERE OrganizationId = @pOrganizationId
    ORDER BY MenuOrder, MenuName
END