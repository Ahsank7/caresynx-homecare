CREATE PROCEDURE [dbo].[uspGetUserFranchises]
    @pUserId UNIQUEIDENTIFIER,
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all franchises accessible by a user within an organization
    SELECT uf.FranchiseId
    FROM tblUser u
    INNER JOIN tbUserFranchise uf ON u.Id = uf.UserId
    INNER JOIN tblFranchise f ON uf.FranchiseId = f.Id
    WHERE u.Id = @pUserId 
    AND f.OrganizationId = @pOrganizationId
    AND u.IsActive = 1
    AND uf.IsActive = 1
    AND f.IsActive = 1;
END