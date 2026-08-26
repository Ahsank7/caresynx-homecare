CREATE OR ALTER PROCEDURE [dbo].[uspUpdateOrganizationBillingSettings]
    @pOrganizationId UNIQUEIDENTIFIER,
    @pServiceRateForBilling INT,
    @pDefaultBillingRate DECIMAL(10,2),
    @pDefaultWageRate DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[tblOrganization]
    SET 
        ServiceRateForBilling = @pServiceRateForBilling,
        DefaultBillingRate = @pDefaultBillingRate,
        DefaultWageRate = @pDefaultWageRate,
        UpdatedAt = SYSUTCDATETIME()
    WHERE Id = @pOrganizationId;
END
