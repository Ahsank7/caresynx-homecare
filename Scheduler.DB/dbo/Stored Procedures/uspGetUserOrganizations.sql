CREATE PROCEDURE [dbo].[uspGetUserOrganizations]
    @pUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all organizations accessible by a user
    SELECT DISTINCT f.OrganizationId
    FROM tblUser u
    INNER JOIN tbUserFranchise uf ON u.Id = uf.UserId
    INNER JOIN tblFranchise f ON uf.FranchiseId = f.Id
    WHERE u.Id = @pUserId 
    AND u.IsActive = 1
    AND uf.IsActive = 1
    AND f.IsActive = 1;
END