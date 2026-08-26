CREATE   PROCEDURE [dbo].[CreateServiceType]
    @pName NVARCHAR(100),
	@pDescription NVARCHAR(500),
	@pOrganizationId uniqueidentifier
AS
BEGIN
    INSERT INTO tblServicesType (Name,Description,OrganizationId,IsActive)
    VALUES (@pName,@pDescription,@pOrganizationId,1);

    SELECT TOP 1 * FROM tblServicesType WHERE Id = SCOPE_IDENTITY();
END