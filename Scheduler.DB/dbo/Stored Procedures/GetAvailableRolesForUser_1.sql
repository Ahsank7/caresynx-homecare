
CREATE PROCEDURE [dbo].[GetAvailableRolesForUser]
    @pOrganizationId UNIQUEIDENTIFIER = NULL,
    @pCurrentUserId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get the current user's role level
    DECLARE @vCurrentUserRoleLevel INT;
    
    IF @pCurrentUserId IS NOT NULL
    BEGIN
        -- MIN(RoleLevel) = highest authority if user has multiple roles (lower number = higher authority)
        SELECT @vCurrentUserRoleLevel = MIN(r.RoleLevel)
        FROM tblUserRole ur
        INNER JOIN tblRole r ON ur.RoleId = r.Id
        WHERE ur.UserId = @pCurrentUserId 
        AND ur.IsActive = 1
        AND r.IsActive = 1;
    END
    
    -- If user's role level is not found, return empty result
    -- This prevents unauthorized access
    IF @vCurrentUserRoleLevel IS NULL
    BEGIN
        SELECT 
            Id,
            Name,
            Description,
            OrganizationId,
            IsActive,
            CreatedDate,
            CreatedBy,
            UpdatedDate,
            UpdatedBy,
            RoleLevel
        FROM tblRole 
        WHERE 1 = 0; -- Return no rows
        RETURN;
    END
    
    -- Get system-wide roles and organization-specific roles
    -- Only return roles with RoleLevel > current user's RoleLevel
    -- (Higher number = Lower authority, so user can only assign lower authority roles)
    SELECT 
        Id,
        Name,
        Description,
        OrganizationId,
        IsActive,
        CreatedDate,
        CreatedBy,
        UpdatedDate,
        UpdatedBy,
        RoleLevel
    FROM tblRole 
    WHERE IsActive = 1 
    AND (OrganizationId IS NULL OR OrganizationId = @pOrganizationId)
    AND RoleLevel > @vCurrentUserRoleLevel  -- Strictly lower authority only (higher RoleLevel number)
    ORDER BY RoleLevel, Name;  -- Order by hierarchy then name
END