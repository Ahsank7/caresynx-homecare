CREATE PROCEDURE [dbo].[uspGetOrganizationTimeZone]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get the timezone for the organization
    SELECT 
        [TimeZone]
    FROM [dbo].[tblOrganization] 
    WHERE [Id] = @pOrganizationId 
        AND [IsActive] = 1;
    
    -- If no organization found or timezone is null, return default
    IF @@ROWCOUNT = 0 OR (SELECT [TimeZone] FROM [dbo].[tblOrganization] WHERE [Id] = @pOrganizationId) IS NULL
    BEGIN
        SELECT 'Pakistan Standard Time' AS [TimeZone];
    END
END
