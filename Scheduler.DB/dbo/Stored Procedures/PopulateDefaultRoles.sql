CREATE   PROCEDURE [dbo].[PopulateDefaultRoles]
    @pOrganizationId uniqueidentifier = null
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if roles already exist for this organization
    IF EXISTS (SELECT 1 FROM tblRole WHERE OrganizationId = @pOrganizationId)
    BEGIN
        PRINT 'Roles already exist for this organization. Skipping population.';
        RETURN;
    END

    -- Insert default roles (RoleLevel: lower number = higher authority; aligns with sp_Seed_MasterData / tblRole)
    INSERT INTO tblRole (Name, Description, OrganizationId, IsActive, CreatedDate, CreatedBy, RoleLevel) VALUES
    ('Administrator', 'Full access to all features and settings', @pOrganizationId, 1, GETUTCDATE(), @pOrganizationId, 2),
    ('Manager', 'Access to manage staff, clients, and service providers', @pOrganizationId, 1, GETUTCDATE(), @pOrganizationId, 3),
    ('Supervisor', 'Access to oversee daily operations and staff', @pOrganizationId, 1, GETUTCDATE(), @pOrganizationId, 4),
    ('Staff', 'Basic access to assigned tasks and limited features', @pOrganizationId, 1, GETUTCDATE(), @pOrganizationId, 6),
    ('Viewer', 'Read-only access to view information only', @pOrganizationId, 1, GETUTCDATE(), @pOrganizationId, 7);

    PRINT 'Default roles populated successfully!';
END