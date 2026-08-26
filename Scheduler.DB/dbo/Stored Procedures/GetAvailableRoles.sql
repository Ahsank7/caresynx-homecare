CREATE PROCEDURE [dbo].[GetAvailableRoles]
    @pOrganizationId uniqueidentifier = null
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get system-wide roles and organization-specific roles
    SELECT 
        Id,
        Name,
        Description,
        OrganizationId,
        IsActive,
        CreatedDate,
        CreatedBy,
        UpdatedDate,
        UpdatedBy
    FROM tblRole 
    WHERE IsActive = 1 
    AND (OrganizationId IS NULL OR OrganizationId = @pOrganizationId)
    ORDER BY Name
END