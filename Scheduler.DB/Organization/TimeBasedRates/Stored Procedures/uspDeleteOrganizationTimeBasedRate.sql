-- Delete time-based rate for an organization
CREATE OR ALTER PROCEDURE [dbo].[uspDeleteOrganizationTimeBasedRate]
    @pId INT,
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate input parameters
        IF @pId IS NULL OR @pId <= 0
        BEGIN
            RAISERROR('Valid Id is required', 16, 1);
            RETURN;
        END
        
        IF @pOrganizationId IS NULL
        BEGIN
            RAISERROR('OrganizationId is required', 16, 1);
            RETURN;
        END
        
        -- Delete the record
        DELETE FROM [dbo].[tblOrganizationTimeBasedRates]
        WHERE Id = @pId AND OrganizationId = @pOrganizationId;
        
        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Time-based rate not found or access denied', 16, 1);
            RETURN;
        END
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        THROW;
    END CATCH
END
