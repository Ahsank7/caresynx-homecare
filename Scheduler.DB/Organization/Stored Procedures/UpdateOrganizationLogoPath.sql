CREATE PROCEDURE [Organization].[UpdateOrganizationLogoPath]
    @pOrganizationId UNIQUEIDENTIFIER,
    @pLogoPath NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[tblOrganization]
    SET LogoPath = @pLogoPath
    WHERE Id = @pOrganizationId;

    -- Return 1 for success, 0 for failure
    IF @@ROWCOUNT > 0
        SELECT CAST(1 AS BIT) AS Success;
    ELSE
        SELECT CAST(0 AS BIT) AS Success;
END