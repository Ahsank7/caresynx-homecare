CREATE    PROCEDURE [dbo].[UpdateService]
    @pId INT,
    @pName NVARCHAR(100),
	@pDescription NVARCHAR(500),
    @pServiceTypeId INT,
    @pRate DECIMAL(18, 2)
AS
BEGIN
    UPDATE tblServices
    SET Name = @pName,
	    [Description]=@pDescription,
        ServiceTypeId = @pServiceTypeId,
        Rate = @pRate
    WHERE Id = @pId;

    SELECT * FROM tblServices WHERE Id = @pId;
END