CREATE PROCEDURE [dbo].[UpdateMenuVisibility]
    @pMenuId NVARCHAR(100),
    @pIsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE tblMenu
    SET IsActive = @pIsActive
    WHERE MenuId = @pMenuId;

    SELECT @@ROWCOUNT AS AffectedRows;
END