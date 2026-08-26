CREATE PROCEDURE [dbo].[uspGetUserFranchiseAssignments]
    @pUserId UNIQUEIDENTIFIER,
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all franchise assignments for a user within an organization
    SELECT 
        uf.Id,
        uf.UserId,
        uf.FranchiseId,
        f.Name AS FranchiseName,
        uf.IsActive
    INTO #FinalResults
    FROM tbUserFranchise uf
    INNER JOIN tblFranchise f ON uf.FranchiseId = f.Id
    WHERE uf.UserId = @pUserId 
    AND f.OrganizationId = @pOrganizationId
    AND f.IsActive = 1;

    -- Return data and count (required for GetAll<> method)
    SELECT * FROM #FinalResults ORDER BY FranchiseName;
    SELECT COUNT(*) AS TotalRecords FROM #FinalResults;
END

