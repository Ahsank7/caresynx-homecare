
CREATE   PROCEDURE [dbo].[GetAllMenus]
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
    WHERE IsActive = 1 and OrganizationId=@pOrganizationId
    ORDER BY MenuOrder, MenuName
END