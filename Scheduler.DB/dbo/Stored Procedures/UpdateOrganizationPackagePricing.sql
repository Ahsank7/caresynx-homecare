CREATE PROCEDURE [dbo].[UpdateOrganizationPackagePricing]
    @pOrganizationPackageId UNIQUEIDENTIFIER,
    @pPerClientCharge DECIMAL(18, 2) = NULL,
    @pInitialOneTimeCost DECIMAL(18, 2) = NULL,
    @pInfrastructureCost DECIMAL(18, 2) = NULL,
    @pSupportCharges DECIMAL(18, 2) = NULL,
    @pNewFeatureReportCharges DECIMAL(18, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Only allow updating active packages
    UPDATE [dbo].[tblOrganizationPackage]
    SET 
        PerClientCharge = ISNULL(@pPerClientCharge, PerClientCharge),
        InitialOneTimeCost = ISNULL(@pInitialOneTimeCost, InitialOneTimeCost),
        InfrastructureCost = ISNULL(@pInfrastructureCost, InfrastructureCost),
        SupportCharges = ISNULL(@pSupportCharges, SupportCharges),
        NewFeatureReportCharges = ISNULL(@pNewFeatureReportCharges, NewFeatureReportCharges)
    WHERE Id = @pOrganizationPackageId
      AND IsActive = 1;
    
    -- Return success indicator
    SELECT @@ROWCOUNT AS RowsAffected;
END
