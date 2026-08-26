CREATE   PROCEDURE [dbo].[DeleteServiceType]
    @pId INT
AS
BEGIN
     UPDATE tblServicesType
     SET IsActive = 0
	 WHERE Id = @pId;
END