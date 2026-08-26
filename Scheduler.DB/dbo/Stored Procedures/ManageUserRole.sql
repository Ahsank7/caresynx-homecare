CREATE PROCEDURE [dbo].[ManageUserRole]
    @pUserId UNIQUEIDENTIFIER,
    @pRoleId INT,
    @pCreatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate user type - only UserType 3 (Staff) can have roles
    DECLARE @vUserType INT;
    SELECT @vUserType = UserType 
    FROM tblUser 
    WHERE Id = @pUserId;
    
    IF @vUserType IS NULL
    BEGIN
        RAISERROR('User not found', 16, 1);
        RETURN;
    END;
    
    IF @vUserType != 3
    BEGIN
        RAISERROR('Roles can only be assigned to Staff users (UserType = 3)', 16, 1);
        RETURN;
    END;
    
    -- Validate role exists
    IF NOT EXISTS (SELECT 1 FROM tblRole WHERE Id = @pRoleId AND IsActive = 1)
    BEGIN
        RAISERROR('Invalid role ID', 16, 1);
        RETURN;
    END;
    
    -- Role Hierarchy Validation
    -- Get the role level of the assigner (CreatedBy user)
    DECLARE @vAssignerRoleLevel INT;
    DECLARE @vTargetRoleLevel INT;
    DECLARE @vCurrentUserRoleLevel INT;
    
    IF @pCreatedBy IS NOT NULL
    BEGIN
        -- Get assigner's highest authority (MIN RoleLevel = lower number = higher authority)
        SELECT @vAssignerRoleLevel = MIN(r.RoleLevel)
        FROM tblUserRole ur
        INNER JOIN tblRole r ON ur.RoleId = r.Id
        WHERE ur.UserId = @pCreatedBy 
        AND ur.IsActive = 1
        AND r.IsActive = 1;
        
        -- Get target role level (the role being assigned)
        SELECT @vTargetRoleLevel = RoleLevel
        FROM tblRole
        WHERE Id = @pRoleId AND IsActive = 1;
        
        -- Target user's strongest active role (for hierarchy check)
        SELECT @vCurrentUserRoleLevel = MIN(r.RoleLevel)
        FROM tblUserRole ur
        INNER JOIN tblRole r ON ur.RoleId = r.Id
        WHERE ur.UserId = @pUserId 
        AND ur.IsActive = 1
        AND r.IsActive = 1;
        
        -- Validation rules:
        -- 1. Assigner must have a role
        IF @vAssignerRoleLevel IS NULL
        BEGIN
            RAISERROR('You do not have a role assigned. Contact your administrator.', 16, 1);
            RETURN;
        END;
        
        -- 2. Cannot assign a role with equal or higher authority (lower or equal RoleLevel number)
        IF @vTargetRoleLevel <= @vAssignerRoleLevel
        BEGIN
            RAISERROR('You cannot assign a role equal to or higher than your own role level.', 16, 1);
            RETURN;
        END;
        
        -- 3. Cannot modify a user who has equal or higher authority (lower or equal RoleLevel number)
        IF @vCurrentUserRoleLevel IS NOT NULL AND @vCurrentUserRoleLevel <= @vAssignerRoleLevel
        BEGIN
            RAISERROR('You cannot modify the role of a user with equal or higher authority.', 16, 1);
            RETURN;
        END;
    END;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Deactivate any existing active role for this user
        UPDATE tblUserRole 
        SET IsActive = 0, UpdatedDate = GETUTCDATE()
        WHERE UserId = @pUserId AND IsActive = 1;
        
        -- Insert new role assignment
        INSERT INTO [dbo].[tblUserRole] 
            (Id, UserId, RoleId, IsActive, CreatedDate, CreatedBy)
        VALUES 
            (NEWID(), @pUserId, @pRoleId, 1, GETUTCDATE(), @pCreatedBy);
        
        COMMIT TRANSACTION;
        
        SELECT 'Role assigned successfully' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Rethrow error with original info
        THROW;
    END CATCH
END;