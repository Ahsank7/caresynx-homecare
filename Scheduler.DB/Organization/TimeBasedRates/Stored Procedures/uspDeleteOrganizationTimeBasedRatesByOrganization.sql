CREATE OR ALTER PROCEDURE [dbo].[uspDeleteOrganizationTimeBasedRatesByOrganization]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [dbo].[tblOrganizationTimeBasedRates]
    WHERE OrganizationId = @pOrganizationId;
END
