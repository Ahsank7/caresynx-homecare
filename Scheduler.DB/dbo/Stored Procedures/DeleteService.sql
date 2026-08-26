CREATE    PROCEDURE [dbo].[DeleteService]
    @pId INT
AS
BEGIN
     UPDATE tblServices
     SET IsActive = 0
	 WHERE Id = @pId;
END