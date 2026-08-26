CREATE   PROCEDURE [dbo].[SaveRolePermissions]
    @pRoleId int,
    @pOrganizationId uniqueidentifier,
    @pPermissions nvarchar(max), -- JSON array of permissions
    @pCreatedBy uniqueidentifier
AS
BEGIN
    SET NOCOUNT ON;

    -- Debug: Print the received JSON length and first part
    DECLARE @jsonLength int = LEN(@pPermissions);
    PRINT 'Received permissions JSON length: ' + CAST(@jsonLength AS varchar(10));
    PRINT 'First 500 characters: ' + LEFT(@pPermissions, 500);
    PRINT 'RoleId: ' + CAST(@pRoleId AS varchar(10));
    PRINT 'OrganizationId: ' + CAST(@pOrganizationId AS varchar(50));
    
    -- Validate JSON format
    IF ISJSON(@pPermissions) = 0
    BEGIN
        PRINT 'ERROR: Invalid JSON format received';
        PRINT 'JSON validation failed. Please check the data being sent.';
        THROW 50000, 'Invalid JSON format received', 1;
    END

    BEGIN TRANSACTION;

    begin TRY
        -- Delete existing permissions for this role and organization
        DELETE FROM tblRolePermission 
        WHERE RoleId = @pRoleId 
        AND (OrganizationId = @pOrganizationId OR OrganizationId IS NULL);

        -- Debug: Check JSON structure
        DECLARE @jsonCount int = (SELECT COUNT(*) FROM OPENJSON(@pPermissions));
        PRINT 'JSON array contains ' + CAST(@jsonCount AS varchar(10)) + ' items';
        
        -- Debug: Print first few JSON items
        SELECT TOP 3 
            JSON_VALUE(value, '$.MenuId') as MenuId,
            JSON_VALUE(value, '$.CanView') as CanView,
            JSON_VALUE(value, '$.CanCreate') as CanCreate,
            JSON_VALUE(value, '$.CanEdit') as CanEdit,
            JSON_VALUE(value, '$.CanDelete') as CanDelete
        FROM OPENJSON(@pPermissions);

        -- Insert new permissions - only extract the fields that exist in tblRolePermission
        INSERT INTO tblRolePermission (Id, RoleId, MenuId, CanView, CanCreate, CanEdit, CanDelete, OrganizationId, CreatedBy)
        SELECT 
            NEWID(),
            @pRoleId,
            COALESCE(
                JSON_VALUE(value, '$.MenuId'),
                JSON_VALUE(value, '$.menuId'),
                JSON_VALUE(value, '$.menuid')
            ) as MenuId,
            ISNULL(CAST(JSON_VALUE(value, '$.CanView') AS bit), 1) as CanView,
            ISNULL(CAST(JSON_VALUE(value, '$.CanCreate') AS bit), 0) as CanCreate,
            ISNULL(CAST(JSON_VALUE(value, '$.CanEdit') AS bit), 0) as CanEdit,
            ISNULL(CAST(JSON_VALUE(value, '$.CanDelete') AS bit), 0) as CanDelete,
            @pOrganizationId,
            @pCreatedBy
        FROM OPENJSON(@pPermissions)
        WHERE COALESCE(
            JSON_VALUE(value, '$.MenuId'),
            JSON_VALUE(value, '$.menuId'),
            JSON_VALUE(value, '$.menuid')
        ) IS NOT NULL;

        -- Debug: Print inserted count
        DECLARE @insertedCount int = @@ROWCOUNT;
        PRINT 'Successfully inserted ' + CAST(@insertedCount AS varchar(10)) + ' permissions';
        
        -- Debug: Verify inserted data
        SELECT COUNT(*) as TotalPermissions FROM tblRolePermission 
        WHERE RoleId = @pRoleId AND OrganizationId = @pOrganizationId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Enhanced error information
        DECLARE @errorMessage nvarchar(4000) = ERROR_MESSAGE();
        DECLARE @errorLine int = ERROR_LINE();
        DECLARE @errorProcedure nvarchar(128) = ERROR_PROCEDURE();
        
        PRINT 'Error in ' + @errorProcedure + ' at line ' + CAST(@errorLine AS varchar(10));
        PRINT 'Error message: ' + @errorMessage;
        PRINT 'JSON length: ' + CAST(@jsonLength AS varchar(10));
        
        THROW;
    END CATCH
END