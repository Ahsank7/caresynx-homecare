CREATE OR ALTER PROCEDURE [dbo].[uspGetOrganizationBillingSettings]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id AS OrganizationId,
        Name AS OrganizationName,
        ServiceRateForBilling,
        DefaultBillingRate,
        DefaultWageRate
    FROM [dbo].[tblOrganization]
    WHERE Id = @pOrganizationId;
END
