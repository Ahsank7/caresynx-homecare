CREATE   PROCEDURE [dbo].[CreateService]
    @pName NVARCHAR(100),
	@pDescription NVARCHAR(500),
    @pServiceTypeId INT,
    @pRate DECIMAL(18, 2) = 0
AS
BEGIN
    INSERT INTO tblServices(Name, Description, ServiceTypeId, Rate, IsActive)
    VALUES (@pName, @pDescription, @pServiceTypeId, @pRate, 1);

    SELECT TOP 1 * FROM tblServices WHERE Id = SCOPE_IDENTITY();
END