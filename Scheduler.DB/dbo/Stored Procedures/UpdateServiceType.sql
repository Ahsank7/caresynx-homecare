CREATE   PROCEDURE [dbo].[UpdateServiceType]
    @pId INT,
    @pName NVARCHAR(100),
	@pDescription NVARCHAR(500)
AS
BEGIN
    UPDATE tblServicesType
    SET [Name] = @pName, [Description]=@pDescription
    WHERE Id = @pId;

    SELECT * FROM tblServicesType WHERE Id = @pId;
END